﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace HlidacStatu.Api.Dataset.Connector
{
    public class DatasetConnector : IDatasetConnector
    {
        private string apiRoot = "https://www.hlidacstatu.cz/api/v1";

        private readonly string ApiToken;
        private readonly HttpClient HttpClient;

        public void SetDeveleperUrl(string devApiUrl = null)
        {
            apiRoot = devApiUrl ?? "http://local.hlidacstatu.cz/api/v1";
        }

        public DatasetConnector(string apiToken)
        {
            ApiToken = apiToken;

            HttpClient = new HttpClient();
            HttpClient.DefaultRequestHeaders.Add("Authorization", ApiToken);
        }

        public enum AddItemMode
        {
            Skip,
            Rewrite,
            Merge
        }
        public async Task<string> AddItemToDataset<TData>(Dataset<TData> dataset, TData item, AddItemMode mode = AddItemMode.Skip)
            where TData : IDatasetItem
        {
            return await AddItemToDataset<TData>(dataset.DatasetId, item, mode);
        }

        public async Task<string> AddItemToDataset<TData>(string datasetId, TData item, AddItemMode mode = AddItemMode.Skip)
            where TData : IDatasetItem
        {
            var content = new StringContent(JsonConvert.SerializeObject(item));
            var response = await HttpClient.PostAsync(apiRoot + $"/DatasetItem/{datasetId}/{item.Id}?mode={mode.ToString()}", content);
            var result = JObject.Parse(await response.Content.ReadAsStringAsync());
            if (result["error"] == null)
            {
                return result["id"].Value<string>();
            }
            else
            {
                throw new DatasetConnectorException(result["error"]["description"].Value<string>());
            }
        }
        public async Task<bool> DatasetExists<TData>(Dataset<TData> dataset)
            where TData : IDatasetItem
        {
            var response = await HttpClient.GetAsync(apiRoot + "/Datasets/" + dataset.DatasetId);
            var result = JObject.Parse(await response.Content.ReadAsStringAsync());

            if (result["error"] == null)
                return true;
            else
                return false;
        }

        public async Task<string> CreateDataset<TData>(Dataset<TData> dataset)
            where TData : IDatasetItem
        {
            if (await DatasetExists(dataset))
            {
                throw new DatasetConnectorException($"Dataset {dataset.DatasetId} already exists");
            }

            var content = new StringContent(JsonConvert.SerializeObject(dataset));
            var response = await HttpClient.PostAsync(apiRoot + "/Datasets", content);
            var result = JObject.Parse(await response.Content.ReadAsStringAsync());
            if (result["error"] == null)
            {
                return result["datasetId"].Value<string>();
            }
            else
            {
                throw new DatasetConnectorException(result["error"]["description"].ToString());
            }
        }

        public async Task<string> UpdateDataset<TData>(Dataset<TData> dataset)
            where TData : IDatasetItem
        {
            if (!await DatasetExists(dataset))
            {
                throw new DatasetConnectorException($"Dataset {dataset.DatasetId} not found");
            }

            var content = new StringContent(JsonConvert.SerializeObject(dataset));
            var response = await HttpClient.PutAsync(apiRoot + "/Datasets/" + dataset.DatasetId, content);
            var result = JObject.Parse(await response.Content.ReadAsStringAsync());

            if (!result["error"].HasValues)
            {
                return "Ok";
            }
            else
            {
                throw new DatasetConnectorException(result["error"]["description"].ToString());
            }
        }

        public async Task DeleteDataset<TData>(Dataset<TData> dataset)
            where TData : IDatasetItem
        {
            if (!await DatasetExists(dataset))
            {
                throw new DatasetConnectorException($"Dataset {dataset.DatasetId} not found");
            }

            var deleteResponse = await HttpClient.DeleteAsync(apiRoot + "/Datasets/" + dataset.DatasetId);
            await deleteResponse.Content.ReadAsStringAsync();
            return;
        }

        public async Task<TData> GetItemFromDataset<TData>(string datasetId, string id) where TData : IDatasetItem
        {
            //var content = new StringContent(JsonConvert.SerializeObject(item));
            var response = await HttpClient.GetAsync(apiRoot + $"/DatasetItem/{datasetId}/{id}");
            var result = JObject.Parse(await response.Content.ReadAsStringAsync());
            if (result["error"] == null)
            {
                return result.ToObject<TData>();
            }
            else
            {
                throw new DatasetConnectorException(result["error"]["description"].Value<string>());
            }
        }


        public async Task<SearchResult<TData>> SearchItemsInDataset<TData>(string datasetId, string query, int page, string sort = "", bool desc = false)
            where TData : IDatasetItem
        {
            //var content = new StringContent(JsonConvert.SerializeObject(item));
            var response = await HttpClient.GetAsync(apiRoot + $"/DatasetSearch/{datasetId}?q={System.Net.WebUtility.UrlEncode(query)}&page={page}&sort={sort}&desc={(desc ? "true" : "false")}");
            var result = JObject.Parse(await response.Content.ReadAsStringAsync());
            if (result["error"] == null)
            {
                return result.ToObject<SearchResult<TData>>();
            }
            else
            {
                throw new DatasetConnectorException(result["error"]["description"].Value<string>());
            }
        }
    }
}
