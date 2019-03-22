using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BiHuManBu.ExternalInterfaces.Models.Model
{
    public class PageNavifationHelper
    {
        public static string ShowPageNavigate(int currentPage, int pageSize, int totalCount)
        {
            TagBuilder builder = new TagBuilder("ul");
            builder.IdAttributeDotReplacement = "_";
            builder.GenerateId("test");
            builder.AddCssClass("pagination");
            builder.InnerHtml = GetNormalPage(currentPage, pageSize, totalCount, PageMode.Numeric);
            return builder.ToString();
        }
        /// <summary>
        /// 分页模式
        /// </summary>
        public enum PageMode
        {
            /// <summary>
            /// 普通分页模式
            /// </summary>
            Normal,
            /// <summary>
            /// 普通分页加数字分页
            /// </summary>
            Numeric
        }

        /// <summary>
        /// 获取普通分页
        /// </summary>
        /// <param name="currentPageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="recordCount"></param>
        /// <param name="goNum"></param>
        /// <returns></returns>
        private static string GetNormalPage(int currentPageIndex, int pageSize, int recordCount, PageMode mode)
        {
            if (recordCount > 0)
            {
                #region
                int pageCount = (recordCount % pageSize == 0 ? recordCount / pageSize : recordCount / pageSize + 1);
                StringBuilder url = new StringBuilder();
                url.Append(HttpContext.Current.Request.Url.AbsolutePath + "?pageIndex={0}");
                NameValueCollection collection = HttpContext.Current.Request.RequestType == "get".ToUpper()
                    ? HttpContext.Current.Request.QueryString
                    : HttpContext.Current.Request.Form;
                //string[] keys = collection.AllKeys;
                //for (int i = 0; i < keys.Length; i++)
                //{
                //    if (keys[i].ToLower() != "pageindex")
                //        url.AppendFormat("&{0}={1}", keys[i], collection[keys[i]]);
                //}
                StringBuilder sb = new StringBuilder();
                if (currentPageIndex == 1)
                {
                    sb.AppendFormat(@"<li>
                                  <a href='#' data-pageindex='{0}' data-url='{1}'  aria-label='Previous' class='pagePrev pageLink'>
                                    <span aria-hidden='true'>首页</span>
                                  </a>
                                </li>", "#", "#");
                }
                else
                {
                    string url1 = string.Format(url.ToString(), 1);
                    sb.AppendFormat(@"<li>
                                  <a href='#' data-pageindex='{0}' data-url='{1}' aria-label='Previous' class='pagePrev pageLink'>
                                    <span aria-hidden='true'>首页</span>
                                  </a>
                                </li>", 1, url1);
                }
                if (currentPageIndex > 1)
                {
                    string url1 = string.Format(url.ToString(), currentPageIndex - 1);
                    sb.AppendFormat(@"<li>
                                  <a href='#' data-pageindex='{0}' data-url='{1}' aria-label='Previous' class='pagePrev0 pageLink'>
                                    <span aria-hidden='true'>上一页</span>
                                  </a>
                                </li>", currentPageIndex - 1, url1);
                }
                else
                {
                    sb.AppendFormat(@"<li>
                                  <a href='#' data-pageindex='{0}' data-url='{1}' aria-label='Previous' class='pagePrev0 pageLink'>
                                    <span aria-hidden='true'>上一页</span>
                                  </a>
                                </li>", "#", "#");
                }
                if (mode == PageMode.Numeric)
                    sb.Append(GetNumericPage(currentPageIndex, pageSize, recordCount, pageCount, url.ToString()));
                if (currentPageIndex < pageCount)
                {
                    string url1 = string.Format(url.ToString(), currentPageIndex + 1);
                    sb.AppendFormat(@"<li>
                                  <a href='#' data-pageindex='{0}' data-url='{1}' aria-label='Previous' class='pagePrev0 pageLink'>
                                    <span aria-hidden='true'>下一页</span>
                                  </a>
                                </li>", currentPageIndex + 1, url1);
                }
                else
                {
                    sb.AppendFormat(@"<li>
                                  <a href='#' data-pageindex='{0}' data-url='{1}' aria-label='Previous' class='pagePrev0 pageLink'>
                                    <span aria-hidden='true'>下一页</span>
                                  </a>
                                </li>", "#", "#");
                }

                if (currentPageIndex == pageCount)
                {
                    sb.AppendFormat(@"<li>
                                  <a href='#' data-pageindex='{0}' data-url='{1}' aria-label='Previous' class='pagePrev pageLink'>
                                    <span aria-hidden='true'>末页</span>
                                  </a>
                                </li>", "#", "#");
                }
                else
                {
                    string url1 = "";
                    if (pageCount == 0)
                    {
                        url1 = string.Format(url.ToString(), 1);
                    }
                    else
                    {
                        url1 = string.Format(url.ToString(), pageCount);
                    }

                    sb.AppendFormat(@"<li>
                                  <a href='#' data-pageindex='{0}' data-url='{1}' aria-label='Previous' class='pagePrev pageLink'>
                                    <span aria-hidden='true'>末页</span>
                                  </a>
                                </li>", pageCount, url1);
                }
                sb.AppendFormat("<div class='col-lg-2' style='width:132px;padding-left: 15px; padding-right:0px;float:left;'>" +
                                    "<div class='input-group'>" +
                                      "<input type='text' name='goNum' class='form-control form-control-padding form-control-height isNumPage' style='min-width:70px;'  placeholder='页码'>" +
                                      "<span class='input-group-btn'>" +
                                       " <button id='bihu-go' class='btn btn-default form-control-padding form-control-height btn-width-40'  type='button'>Go</button>" +
                                      "</span>" +
                                    "</div>" +
                                  "</div>" +
                                  "<div class='col-lg-3' style='width:auto;text-align: left;height: 34px;line-height:34px;padding-left: 0px; padding-right:0px;float:left;'><label style='font-weight: normal;color: black;margin-left:5px;'>总共<span id='js_setting_totalcount'>{0}</span>条记录</label></div>"
                        , recordCount);
                return sb.ToString();
                #endregion
            }
            //<li id='bihu-no-data'>暂无数据</li>
            return "<script type='text/javascript'>$('#bihu-no-data').parents('.table-bihu-tad').css('margin-top','-20px').end().parents('.nav-paging').css('border','1px solid #ddd').css('border-top','none');</script>";
        }
        /// <summary>
        /// 获取数字分页
        /// </summary>
        /// <param name="currentPageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="recordCount"></param>
        /// <param name="pageCount"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        private static string GetNumericPage(int currentPageIndex, int pageSize, int recordCount, int pageCount, string url)
        {
            int standnum = 7;
            int k = currentPageIndex / standnum;
            int m = currentPageIndex % standnum;
            StringBuilder sb = new StringBuilder();
            if (currentPageIndex / standnum == pageCount / standnum)
            {
                if (m == 0)
                {
                    k--;
                    m = standnum;
                }
                else
                    m = pageCount % standnum;
            }
            else
                m = standnum;
            for (int i = k * standnum; i <= k * standnum + m; i++)
            {
                if (i != 0)
                {
                    if (i == currentPageIndex)
                    {
                        string url1 = string.Format(url, i);
                        sb.AppendFormat(@"<li class='active'><a href={0} data-pageindex='{2}' class='pagePrev pageLink' data-url='{3}'>{1}</a></li>", "#", i, i, url1);
                    }
                    else
                    {
                        string url1 = string.Format(url, i);
                        sb.AppendFormat(@"<li><a href='#' data-pageindex='{0}' class='pagePrev pageLink' data-url='{2}'>{1}</a></li>", i, i, url1);
                    }
                }
            }

            return sb.ToString();
        }
    }
}
