using System;
using System.Collections.Generic;

namespace Unity2GPT
{
    public class JsonGPTClass
    {
        [Serializable]
        public class GPTMessageModel
        {
            public string role;
            public string content;
        }

        [Serializable]
        public class GPTCompletionRequestModel
        {
            public string model;
            public List<GPTMessageModel> messages;
        }

        [System.Serializable]
        public class GPTResponseModel
        {
            public string id;
            public string @object;
            public int created;
            public Choice[] choices;
            public Usage usage;

            [System.Serializable]
            public class Choice
            {
                public int index;
                public GPTMessageModel message;
                public string finish_reason;
            }

            [System.Serializable]
            public class Usage
            {
                public int prompt_tokens;
                public int completion_tokens;
                public int total_tokens;
            }
        }
    }
}
