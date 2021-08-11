using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace KeqingNiuza.Wish
{
    public class WishLogExporter
    {
        private readonly string baseRequestUrl = @"https://hk4e-api.mihoyo.com/event/gacha_info/api/getGachaLog";

        private readonly string authString;

        private readonly HttpClient HttpClient;

        public event EventHandler<string> ProgressChanged;

        public WishLogExporter(string url, bool isOversea = false)
        {
            if (url.EndsWith("#/log"))
            {
                authString = url.Substring(url.IndexOf('?')).Replace("#/log", "");
                HttpClient = new HttpClient();
                if (isOversea)
                {
                    baseRequestUrl = @"https://hk4e-api-os.mihoyo.com/event/gacha_info/api/getGachaLog";
                }
            }
            else
            {
                throw new ArgumentException("Url does not meet the requirement!");
            }
        }

        /// <summary>
        /// ��ȡ��Ը��¼����
        /// </summary>
        /// <param name="size">ÿ��Api�����ȡ�������ݣ�Ĭ��6�������20��</param>
        /// <param name="lastId">�������µ�id����ȡ����ԸidС������id��ֹͣ</param>
        /// <returns></returns>
        public async Task<List<WishData>> GetAllLog(int size = 6, long lastId = 0)
        {
            List<WishData> wishDatas = new List<WishData>();
            await Task.Run(() =>
            {
                QueryParam param = new QueryParam() { WishType = WishType.Novice, Page = 1, Size = size, EndId = 0 };
                wishDatas.AddRange(GetWishLogList(param, lastId));
                param = new QueryParam() { WishType = WishType.Permanent, Page = 1, Size = size, EndId = 0 };
                wishDatas.AddRange(GetWishLogList(param, lastId));
                param = new QueryParam() { WishType = WishType.CharacterEvent, Page = 1, Size = size, EndId = 0 };
                wishDatas.AddRange(GetWishLogList(param, lastId));
                param = new QueryParam() { WishType = WishType.WeaponEvent, Page = 1, Size = size, EndId = 0 };
                wishDatas.AddRange(GetWishLogList(param, lastId));
            });
            return wishDatas;
        }


        public List<WishData> GetWishLogList(WishType type)
        {
            QueryParam param = new QueryParam() { WishType = type, Page = 1, Size = 6, EndId = 0 };
            return GetWishLogList(param);
        }


        /// <summary>
        /// ��ȡUrl������Uid
        /// </summary>
        /// <exception cref="Exception">û����Ը��¼</exception>
        /// <returns></returns>
        public async Task<int> GetUidByUrl()
        {
            var list = new List<WishData>();
            await Task.Run(() =>
             {
                 QueryParam param = new QueryParam() { WishType = WishType.Novice, Page = 1, Size = 6, EndId = 0 };
                 list.AddRange(GetWishLog(param));
                 param = new QueryParam() { WishType = WishType.Permanent, Page = 1, Size = 6, EndId = 0 };
                 list.AddRange(GetWishLog(param));
                 param = new QueryParam() { WishType = WishType.CharacterEvent, Page = 1, Size = 6, EndId = 0 };
                 list.AddRange(GetWishLog(param));
                 param = new QueryParam() { WishType = WishType.WeaponEvent, Page = 1, Size = 6, EndId = 0 };
                 list.AddRange(GetWishLog(param));
             });
            if (list.Any())
            {
                return list.First().Uid;
            }
            else
            {
                throw new Exception("û����Ը��¼");
            }
        }


        /// <summary>
        /// ��ȡָ�����͵�������Ը��¼�����ֹ��ָ��id
        /// </summary>
        /// <param name="param">��Ը����</param>
        /// <param name="lastId">��ֹ������Ըid</param>
        /// <returns></returns>
        private List<WishData> GetWishLogList(QueryParam param, long lastId = 0)
        {
            List<WishData> list = new List<WishData>();
            string url, str;
            ResponseData result;
            do
            {
                OnProgressChanged(param);
                url = $@"{baseRequestUrl}{authString}&{param}";
                str = HttpClient.GetStringAsync(url).Result;
                result = JsonSerializer.Deserialize<ResponseData>(str);
                if (result.Retcode != 0)
                {
                    throw new ArgumentException(result.Message);
                }
                if (result.Data.List.Count != 0)
                {
                    list.AddRange(result.Data.List);
                    param.Page++;
                    param.EndId = result.Data.List.Last().Id;
                    if (param.EndId <= lastId)
                    {
                        break;
                    }
                }

            } while (result.Data.List.Count == param.Size);
            return list;
        }

        /// <summary>
        /// ��ȡһҳ��Ը����
        /// </summary>
        /// <param name="param">�������</param>
        /// <returns></returns>
        private List<WishData> GetWishLog(QueryParam param)
        {
            List<WishData> list = new List<WishData>();
            var url = $@"{baseRequestUrl}{authString}&{param}";
            var str = HttpClient.GetStringAsync(url).Result;
            var result = JsonSerializer.Deserialize<ResponseData>(str);
            if (result.Retcode != 0)
            {
                throw new ArgumentException(result.Message);
            }
            if (result.Data.List.Count != 0)
            {
                list.AddRange(result.Data.List);
            }
            return list;
        }


        private void OnProgressChanged(QueryParam param)
        {
            string type = null;
            switch (param.WishType)
            {
                case WishType.Novice:
                    type = "����";
                    break;
                case WishType.Permanent:
                    type = "��פ";
                    break;
                case WishType.CharacterEvent:
                    type = "��ɫ";
                    break;
                case WishType.WeaponEvent:
                    type = "����";
                    break;
            }
            ProgressChanged?.Invoke(this, $"���ڻ�ȡ {type}��Ը �� {param.Page} ҳ");
        }
    }
}
