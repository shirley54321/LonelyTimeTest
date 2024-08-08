using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PlayFab.ClientModels;
using UnityEngine;
using LitJson;

namespace Tool
{
    /// <summary>
    /// Build data for PlayerData, ReadOnlyData in PlayFab
    /// provide debug error message when value is null
    /// </summary>
    public static class PlayFabDataBuilder
    {
        /// <summary>
        /// Build data for PlayerData, ReadOnlyData in PlayFab
        /// </summary>
        /// <param name="key">Key for Data</param>
        /// <param name="dataRecords">The dictionary for Data</param>
        /// <param name="dataType"></param>
        /// <typeparam name="T">The data structure(class) for Data</typeparam>
        /// <returns>The first element in List<T></returns>
        public static T BuildLastElementInList<T>(string key, 
            Dictionary<string, UserDataRecord> dataRecords, PlayFabDataType dataType) where T : new()
        {
            var list = BuildData<List<T>>(key, dataRecords, dataType);
            if (list != null )
            {
                if (list.Count > 0)
                {
                    return list[list.Count - 1];
                }
            }

            return new T();
        }

        /// <summary>
        /// Build data for PlayerData, ReadOnlyData in PlayFab
        /// </summary>
        /// <param name="key">Key for Data</param>
        /// <param name="daraRecords">The dictionary for Data</param>
        /// <param name="dataType"></param>
        /// <typeparam name="T">The data structure(class) for Data</typeparam>
        /// <returns></returns>
        public static T BuildData<T>(string key,  Dictionary<string,UserDataRecord> daraRecords, 
            PlayFabDataType dataType) where T : new()
        {
            T output = new T();
            if (daraRecords.TryGetValue(key, out var value))
            {
                try
                {
                    output = JsonMapper.ToObject<T>(value.Value);
                    if(key=="AccumulateTopUp" || key=="Activity"){
                        Debug.Log($"Deserialize Key: {key}, Value {value.Value}");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Deserialize failed: Key: {key}, Value {value.Value}\n" +
                                   $"error: {e.Message}");
                }
            }
            else
            {
                Debug.LogError($"{dataType} doesn't contain {key}, " +
                               $"please generate this data in PlayFab Dashboard");
            }
            return output;
        }

        public static int BuildDataInt(string key, Dictionary<string, UserDataRecord> dataRecords, PlayFabDataType dataType)
        {
            if (dataRecords.TryGetValue(key, out var value))
            {
                if (int.TryParse(value.Value, out int intValue))
                {
                    //Debug.Log($"Deserialize Key: {key}, Value {value.Value}");
                    return intValue;
                }
                else
                {
                    Debug.LogError($"Failed to parse value to integer: Key: {key}, Value: {value.Value}");
                }
            }
            else
            {
                Debug.LogError($"{dataType} doesn't contain {key}, please generate this data in PlayFab Dashboard");
            }
            return 0;
        }


    }

    public enum PlayFabDataType
    {
        PlayerData,
        ReadOnlyData
    }
}