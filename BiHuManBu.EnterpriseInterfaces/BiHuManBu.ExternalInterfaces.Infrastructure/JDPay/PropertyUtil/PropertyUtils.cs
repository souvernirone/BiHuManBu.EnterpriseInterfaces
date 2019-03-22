﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BiHuManBu.ExternalInterfaces.Infrastructure.JDPay.PropertyUtil
{
    public class PropertyUtils
    {
        public static Dictionary<String, String> propertyDictionary = new Dictionary<String, String>();
        static PropertyUtils()
        {

            propertyDictionary.AddOrPeplace("wepay.merchant.num", "22294531");
            propertyDictionary.AddOrPeplace("wepay.merchant.rsaPrivateKey", "MIICdgIBADANBgkqhkiG9w0BAQEFAASCAmAwggJcAgEAAoGBALXf6twUqul1TATO+5nA66p2wjnRd+g96IXpfV6Sf8WXxwizGj+L19LQYRBXpZHmRh82prJ48d0FcHboCiN8pKutnuZrrKYhvORysOc5bVli0hcCn1TfYDoUWJ1UhjUQloqZKWjUz6LV9QY6bIZ1W4+Hmw6HK1bfFwUq0WzIGkJNAgMBAAECgYBlIFQeev9tP+M86TnMjBB9f/sO2wGpCIM5slIbO6n/3By3IZ7+pmsitOrDg3h0X22t/V1C7yzMkDGwa+T3Rl7ogwc4UNVj0ZQorOTx3OEPx3nP1yT3zmJ9djKaHKAmee4XmhQHdqqIuMT2XQaqatBzcsnP+Jnw/WVOsIJIqMeFAQJBAP9yq4hE+UfM/YSXZ5JR33k9RolUUq8S/elmeJIDo/3N2qDmzLjOr9iEZHxioc8JOxubtZ0BxA+NdfKz4v0BSpkCQQC2RIrAPRj9vOk6GfT9W1hbJ4GdnzTb+4vp3RDQQ3x9JGXzWFlg8xJT1rNgM8R95Gkxn3KGnYHJQTLlCsIy2FnVAkAWXolM3pVhxz6wHL4SHx9Ns6L4payz7hrUFIgcaTs0H5G0o2FsEZVuhXFzPwPiaHGHomQOAriTkBSzEzOeaj2JAkEAtYUFefZfETQ2QbrgFgIGuKFboJKRnhOif8G9oOvU6vx43CS8vqTVN9G2yrRDl+0GJnlZIV9zhe78tMZGKUT2EQJAHQawBKGlXlMe49Fo24yOy5DvKeohobjYqzJAtbqaAH7iIQTpOZx91zUcL/yG4dWS6r+wGO7Z1RKpupOJLKG3lA==");
            propertyDictionary.AddOrPeplace("wepay.merchant.desKey", "ta4E/aspLA3lgFGKmNDNRYU92RkZ4w2t");

            propertyDictionary.AddOrPeplace("wepay.jd.rsaPublicKey", "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCKE5N2xm3NIrXON8Zj19GNtLZ8xwEQ6uDIyrS3S03UhgBJMkGl4msfq4Xuxv6XUAN7oU1XhV3/xtabr9rXto4Ke3d6WwNbxwXnK5LSgsQc1BhT5NcXHXpGBdt7P8NMez5qGieOKqHGvT0qvjyYnYA29a8Z4wzNR7vAVHp36uD5RwIDAQAB");
            propertyDictionary.AddOrPeplace("wepay.server.query.url", "http://paygate.jd.com/service/query");
            propertyDictionary.AddOrPeplace("wepay.server.refund.url", "http://paygate.jd.com/service/refund");

            propertyDictionary.AddOrPeplace("wepay.server.query.refund.url", "http://paygate.jd.com/service/queryRefund");
            propertyDictionary.AddOrPeplace("wepay.server.uniorder.url", "http://paygate.jd.com/service/uniorder");
        }

        public static String getProperty(String key)
        {
            return propertyDictionary.getVaule(key);
        }
    }
      
    
}