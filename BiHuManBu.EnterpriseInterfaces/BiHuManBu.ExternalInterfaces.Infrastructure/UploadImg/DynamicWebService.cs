using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Description;
using Microsoft.CSharp;

namespace BiHuManBu.ExternalInterfaces.Infrastructure.UploadImg
{
    public class DynamicWebService
    {
        /**/
        /// <summary>  
        /// 動態呼叫Web Service 
        /// </summary>  
        /// <param name="pUrl">WebService的http形式的位址，EX:http://www.yahoo.com/Service/Service.asmx </param>  
        /// <param name="pNamespace">欲呼叫的WebService的namespace</param>  
        /// <param name="pClassname">欲呼叫的WebService的class name</param>  
        /// <param name="pMethodname">欲呼叫的WebService的method name</param>  
        /// <param name="pArgs">參數列表，請將每個參數分別放入object[]中</param>  
        /// <returns>WebService的執行結果</returns>  
        /// <remarks>  
        /// 如果呼叫失敗，將會拋出Exception。請呼叫的時候，適當截獲異常。  
        /// 目前知道有兩個地方可能會發生異常：  
        /// 1、動態構造WebService的時候，CompileAssembly失敗。  
        /// 2、WebService本身執行失敗。  
        /// </remarks>  
        public object InvokeWebservice(string pUrl, string @pNamespace, string pClassname, string pMethodname, object[] pArgs)
        {
            WebClient tWebClient = new WebClient();
            //讀取WSDL檔，確認Web Service描述內容 
            Stream tStream = tWebClient.OpenRead(pUrl + "?WSDL");
            ServiceDescription tServiceDesp = ServiceDescription.Read(tStream);
            //將讀取到的WSDL檔描述import近來 
            ServiceDescriptionImporter tServiceDespImport = new ServiceDescriptionImporter();
            tServiceDespImport.AddServiceDescription(tServiceDesp, "", "");
            CodeNamespace tCodeNamespace = new CodeNamespace(@pNamespace);
            //指定要編譯程式 
            CodeCompileUnit tCodeComUnit = new CodeCompileUnit();
            tCodeComUnit.Namespaces.Add(tCodeNamespace);
            tServiceDespImport.Import(tCodeNamespace, tCodeComUnit);

            //以C#的Compiler來進行編譯 
            CSharpCodeProvider tCSProvider = new CSharpCodeProvider();
            ICodeCompiler tCodeCom = tCSProvider.CreateCompiler();

            //設定編譯參數 
            System.CodeDom.Compiler.CompilerParameters tComPara = new
System.CodeDom.Compiler.CompilerParameters();
            tComPara.GenerateExecutable = false;
            tComPara.GenerateInMemory = true;

            //取得編譯結果 
            System.CodeDom.Compiler.CompilerResults tComResult =
tCodeCom.CompileAssemblyFromDom(tComPara, tCodeComUnit);

            //如果編譯有錯誤的話，將錯誤訊息丟出 
            if (true == tComResult.Errors.HasErrors)
            {
                System.Text.StringBuilder tStr = new System.Text.StringBuilder();
                foreach (System.CodeDom.Compiler.CompilerError tComError in tComResult.Errors)
                {
                    tStr.Append(tComError.ToString());
                    tStr.Append(System.Environment.NewLine);
                }
                throw new Exception(tStr.ToString());
            }

            //取得編譯後產出的Assembly 
            System.Reflection.Assembly tAssembly = tComResult.CompiledAssembly;
            Type tType = tAssembly.GetType(@pNamespace + "." + pClassname, true, true);
            object tTypeInstance = Activator.CreateInstance(tType);
            //若WS有overload的話，需明確指定參數內容 
            Type[] tArgsType = null;
            if (pArgs == null)
            {
                tArgsType = new Type[0];
            }
            else
            {
                int tArgsLength = pArgs.Length;
                tArgsType = new Type[tArgsLength];
                for (int i = 0; i < tArgsLength; i++)
                {
                    tArgsType[i] = pArgs[i].GetType();
                }
            }

            //若沒有overload的話，第二個參數便不需要，這邊要注意的是WsiProfiles.BasicProfile1_1本身不支援Web Service overload，因此需要改成不遵守WsiProfiles.BasicProfile1_1協議 
            System.Reflection.MethodInfo tInvokeMethod = tType.GetMethod(pMethodname, tArgsType);
            //實際invoke該method 
            return tInvokeMethod.Invoke(tTypeInstance, pArgs);
        }
    }
}
