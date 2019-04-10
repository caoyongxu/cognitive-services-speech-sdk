//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.CognitiveServices.Speech.Internal;
using static Microsoft.CognitiveServices.Speech.Internal.SpxExceptionThrower;

namespace Microsoft.CognitiveServices.Speech
{
    /// <summary>
    /// Speech translation configuration.
    /// </summary>
    public sealed class SpeechTranslationConfig : SpeechConfig
    {
        private readonly object configLock = new object();
        private string targetLanguages = string.Empty;

        internal SpeechTranslationConfig(IntPtr configPtr) : base(configPtr)
        {
        }

        /// <summary>
        /// Creates an instance of speech translation config with specified subscription key and region.
        /// </summary>
        /// <param name="subscriptionKey">The subscription key, can be empty if authorization token is specified later.</param>
        /// <param name="region">The region name (see the <a href="https://aka.ms/csspeech/region">region page</a>).</param>
        /// <returns>A speech config instance.</returns>
        public new static SpeechTranslationConfig FromSubscription(string subscriptionKey, string region)
        {
            IntPtr config = IntPtr.Zero;
            ThrowIfFail(Internal.SpeechConfig.speech_config_from_subscription(out config, subscriptionKey, region));
            return new SpeechTranslationConfig(config);
        }

        /// <summary>
        /// Creates an instance of the speech translation config with specified authorization token and region.
        /// Note: The caller needs to ensure that the authorization token is valid. Before the authorization token
        /// expires, the caller needs to refresh it by calling this setter on the corresponding
        /// recognizer with a new valid token.
        /// </summary>
        /// <param name="authorizationToken">The authorization token.</param>
        /// <param name="region">The region name (see the <a href="https://aka.ms/csspeech/region">region page</a>).</param>
        /// <returns>A speech config instance.</returns>
        public new static SpeechTranslationConfig FromAuthorizationToken(string authorizationToken, string region)
        {
            IntPtr config = IntPtr.Zero;
            ThrowIfFail(Internal.SpeechConfig.speech_config_from_authorization_token(out config, authorizationToken, region));
            return new SpeechTranslationConfig(config);
        }

        /// <summary>
        /// Creates an instance of the speech translation config with specified endpoint and subscription key.
        /// This method is intended only for users who use a non-standard service endpoint or parameters.
        /// Note: The query parameters specified in the endpoint URI are not changed, even if they are set by any other APIs.
        /// For example, if the recognition language is defined in URI as query parameter "language=de-DE", and the property SpeechRecognitionLanguage is set to "en-US",
        /// the language setting in URI takes precedence, and the effective language is "de-DE".
        /// Only the parameters that are not specified in the endpoint URI can be set by other APIs.
        /// Note: To use an authorization token with FromEndpoint, use FromEndpoint(System.Uri),
        /// and then set the AuthorizationToken property on the created SpeechTranslationConfig instance.
        /// </summary>
        /// <param name="endpoint">The service endpoint to connect to.</param>
        /// <param name="subscriptionKey">The subscription key.</param>
        /// <returns>A SpeechTranslationConfig instance.</returns>
        public new static SpeechTranslationConfig FromEndpoint(Uri endpoint, string subscriptionKey)
        {
            IntPtr config = IntPtr.Zero;
            ThrowIfFail(Internal.SpeechConfig.speech_config_from_endpoint(out config, Uri.EscapeUriString(endpoint.ToString()), subscriptionKey));
            return new SpeechTranslationConfig(config);
        }

        /// <summary>
        /// Creates an instance of the speech translation config with specified endpoint.
        /// This method is intended only for users who use a non-standard service endpoint or parameters.
        /// Note: The query parameters specified in the endpoint URI are not changed, even if they are set by any other APIs.
        /// For example, if the recognition language is defined in URI as query parameter "language=de-DE", and the property SpeechRecognitionLanguage is set to "en-US",
        /// the language setting in URI takes precedence, and the effective language is "de-DE".
        /// Only the parameters that are not specified in the endpoint URI can be set by other APIs.
        /// Note: If the endpoint requires a subscription key for authentication, use FromEndpoint(System.Uri, string) to pass
        /// the subscription key as parameter.
        /// To use an authorization token with FromEndpoint, please use this method to create a SpeechTranslationConfig instance, and then
        /// set the AuthorizationToken property on the created SpeechTranslationConfig instance.
        /// Note: Added in version 1.5.0.
        /// </summary>
        /// <param name="endpoint">The service endpoint to connect to.</param>
        /// <returns>A SpeechTranslationConfig instance.</returns>
        public new static SpeechTranslationConfig FromEndpoint(Uri endpoint)
        {
            IntPtr config = IntPtr.Zero;
            ThrowIfFail(Internal.SpeechConfig.speech_config_from_endpoint(out config, Uri.EscapeUriString(endpoint.ToString()), null));
            return new SpeechTranslationConfig(config);
        }

        /// <summary>
        /// Gets a collection of languages to translate to.
        /// </summary>
        public ReadOnlyCollection<string> TargetLanguages
        {
            get
            {
                var result = new List<string>();

                string[] targetLanguagesArray = new string[0];
                var targetLanguages = progBag.GetProperty(PropertyId.SpeechServiceConnection_TranslationToLanguages);
                if (targetLanguages != string.Empty)
                {
                    targetLanguagesArray = targetLanguages.Split(',');
                }

                for (int i = 0; i < targetLanguagesArray.Length; ++i)
                {
                    result.Add(targetLanguagesArray[i]);
                }

                return new ReadOnlyCollection<string>(result);
            }
        }

        /// <summary>
        /// Add a target languages of translation.
        /// In case when speech synthesis is used and several target languages are specified for translation,
        /// the speech will be synthesized only for the first language.
        /// </summary>
        /// <param name="language"></param>
        public void AddTargetLanguage(string language)
        {
            lock (configLock)
            {
                if (targetLanguages != string.Empty)
                {
                    targetLanguages += ",";
                }
                targetLanguages += language;
            }
            progBag.SetProperty(PropertyId.SpeechServiceConnection_TranslationToLanguages, targetLanguages);
        }

        /// <summary>
        /// Specifies the name of voice tag if a synthesized audio output is desired.
        /// </summary>
        public string VoiceName
        {
            set
            {
                progBag.SetProperty(PropertyId.SpeechServiceConnection_TranslationFeatures, "textToSpeech");
                progBag.SetProperty(PropertyId.SpeechServiceConnection_TranslationVoice, value);
            }
            get
            {
                return progBag.GetProperty(PropertyId.SpeechServiceConnection_TranslationVoice);
            }
        }
    }
}
