using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NJG.Utilities;
using UnityEngine;
using UnityEngine.Networking;

namespace NJG.Runtime.Online
{
    public class LeaderboardClient : Singleton<LeaderboardClient>
    {
        [SerializeField] private string _supabaseUrl;
        [SerializeField] private string _publishableKey;

        private string SubmitUrl => $"{_supabaseUrl}/rest/v1/rpc/submit_ghost_run";
        private string LeaderboardUrl => $"{_supabaseUrl}/rest/v1/ghost_runs";

        [Serializable]
        private class SubmitRunRequest
        {
            public string p_player_name;
            public int p_score;
            public string p_ghost_data;
            public int p_level_seed;
            public string p_game_version;
        }

        [Serializable]
        private class LeaderboardEntryArrayWrapper
        {
            public LeaderboardEntry[] entries;
        }

        public void SubmitRun(string playerName, int score, string ghostData, int levelSeed, Action<bool> onComplete)
        {
            StartCoroutine(SubmitRunRoutine(playerName, score, ghostData, levelSeed, onComplete));
        }

        public void GetTop20(Action<List<LeaderboardEntry>> onComplete)
        {
            StartCoroutine(GetTop20Routine(onComplete));
        }

        public void GetGhostData(string id, Action<string> onComplete)
        {
            StartCoroutine(GetGhostDataRoutine(id, onComplete));
        }

        private IEnumerator SubmitRunRoutine(string playerName, int score, string ghostData, int levelSeed, Action<bool> onComplete)
        {
            SubmitRunRequest data = new()
            {
                p_player_name = playerName,
                p_score = score,
                p_ghost_data = ghostData,
                p_level_seed = levelSeed,
                p_game_version = Application.version
            };

            string json = JsonUtility.ToJson(data);
            byte[] body = Encoding.UTF8.GetBytes(json);

            using UnityWebRequest request = new UnityWebRequest(SubmitUrl, "POST");
            request.uploadHandler = new UploadHandlerRaw(body);
            request.downloadHandler = new DownloadHandlerBuffer();

            AddHeaders(request);
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            bool success = request.result == UnityWebRequest.Result.Success;

            if (!success)
                Debug.LogError($"Submit failed: {request.downloadHandler.text}");

            onComplete?.Invoke(success);
        }

        private IEnumerator GetTop20Routine(Action<List<LeaderboardEntry>> onComplete)
        {
            string url =
                $"{LeaderboardUrl}?select=id,player_name,score,created_at,level_seed&order=score.desc&limit=20";

            using UnityWebRequest request = UnityWebRequest.Get(url);
            AddHeaders(request);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Leaderboard fetch failed: {request.downloadHandler.text}");
                onComplete?.Invoke(new List<LeaderboardEntry>());
                yield break;
            }

            string wrappedJson = $"{{\"entries\":{request.downloadHandler.text}}}";
            LeaderboardEntryArrayWrapper wrapper =
                JsonUtility.FromJson<LeaderboardEntryArrayWrapper>(wrappedJson);

            onComplete?.Invoke(new List<LeaderboardEntry>(wrapper.entries));
        }

        private IEnumerator GetGhostDataRoutine(string id, Action<string> onComplete)
        {
            string url = $"{LeaderboardUrl}?select=ghost_data&id=eq.{id}&limit=1";

            using UnityWebRequest request = UnityWebRequest.Get(url);
            AddHeaders(request);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Ghost fetch failed: {request.downloadHandler.text}");
                onComplete?.Invoke(null);
                yield break;
            }

            string wrappedJson = $"{{\"entries\":{request.downloadHandler.text}}}";
            LeaderboardEntryArrayWrapper wrapper =
                JsonUtility.FromJson<LeaderboardEntryArrayWrapper>(wrappedJson);

            if (wrapper.entries == null || wrapper.entries.Length == 0)
            {
                onComplete?.Invoke(null);
                yield break;
            }

            onComplete?.Invoke(wrapper.entries[0].ghost_data);
        }

        private void AddHeaders(UnityWebRequest request)
        {
            request.SetRequestHeader("apikey", _publishableKey);
            request.SetRequestHeader("Authorization", $"Bearer {_publishableKey}");
        }
    }
}