using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Majaja.Utilities;


namespace SlotTemplate {

    public class AutoplayOptionsPanelTypeA : AutoplayOptionsPanel {

        public int MinBalanceValueOffsetFromPlayerCurrentBalance => _optionsAdjustSettings.minBalanceValueOffsetFromPlayerCurrentBalance;
        public int maxBalanceValueOffsetFromPlayerCurrentBalance => _optionsAdjustSettings.maxBalanceValueOffsetFromPlayerCurrentBalance;


        [SerializeField] OptionsAdjustSettings _optionsAdjustSettings;
        [SerializeField] OptionsAdjustElements _optionsAdjustElements;

        public int minValueTemp = 0;
        public int maxValueTemp = 0;

        public bool isSettingMin { get; set; }
        public bool isSettingMax { get; set; }

        private void Awake()
        {
            SetupCurrentAutoplayOptions();
            _optionsAdjustElements.RetriveSave();
        }
        void Start () {
            _optionsAdjustElements.minBalanceInputField.onEndEdit.AddListener(_optionsAdjustElements.onMinBalanceInputRecieve);
            _optionsAdjustElements.maxBalanceInputField.onEndEdit.AddListener(_optionsAdjustElements.onMaxBalanceInputRecieve);
        }

        public void SetInitAutoplayOptionsMinMaxBalanceValue (int minBalanceValue, int maxBalanceValue) 
        {
            _optionsAdjustElements.MinBalanceValue = _optionsAdjustSettings.minBalanceValueAdjustInfo.GetFinalValue(minBalanceValue);
            _optionsAdjustElements.MaxBalanceValue = _optionsAdjustSettings.maxBalanceValueAdjustInfo.GetFinalValue(maxBalanceValue);
        }

        public void SetSettingStatus(int minStatus, int maxStatus)
        {
            //Debug.Log($"Check in function: Min: {minStatus}, Max: {maxStatus}");
            if (minStatus == 1) { isSettingMin = true; }
            else if (maxStatus == 1) { isSettingMax = true; }
        }

        private void OnDisable()
        {
            minValueTemp = _optionsAdjustElements.MinBalanceValue;
            maxValueTemp = _optionsAdjustElements.MaxBalanceValue;

            _optionsAdjustElements.CheckToggle();
        }
        public void AdjustPlayRoundsValue (int steps) {
            _optionsAdjustElements.playRoundsToggle.isOn = true;
            IntegerValueAdjustInfo info = _optionsAdjustSettings.playRoundsValueAdjustInfo;
            _optionsAdjustElements.PlayRoundsValue = info.GetFinalValue(_optionsAdjustElements.PlayRoundsValue, steps * info.valuePerStep);

            //Save Value
            PlayerPrefs.SetInt("playRoundsToggle", 1);
            PlayerPrefs.SetInt("PlayRoundsValue", _optionsAdjustElements.PlayRoundsValue);
            PlayerPrefs.Save();
        }

        public void AdjustStoppingWonRate (int steps) {
            _optionsAdjustElements.stoppingWonRateToggle.isOn = true;
            IntegerValueAdjustInfo info = _optionsAdjustSettings.stoppingWonRateValueAdjustInfo;
            _optionsAdjustElements.StoppingWonRateValue = info.GetFinalValue(_optionsAdjustElements.StoppingWonRateValue, steps * info.valuePerStep);
            
            //Save Value
            PlayerPrefs.SetInt("stoppingWonRateToggle", 1);
            PlayerPrefs.SetInt("StoppingWonRateValue", _optionsAdjustElements.StoppingWonRateValue);
            PlayerPrefs.Save();
        }

        public void AdjustMinBalance (int steps) {
            _optionsAdjustElements.minBalanceToggle.isOn = true;
            IntegerValueAdjustInfo info = _optionsAdjustSettings.minBalanceValueAdjustInfo;
            _optionsAdjustElements.MinBalanceValue = info.GetFinalValue(_optionsAdjustElements.MinBalanceValue, steps * info.valuePerStep);

            //Save Value
            PlayerPrefs.SetInt("minBalanceToggle", 1);
            PlayerPrefs.SetInt("MinBalanceValue", _optionsAdjustElements.MinBalanceValue);
            PlayerPrefs.Save();
        }

        public void AdjustMaxBalance (int steps) {
            _optionsAdjustElements.maxBalanceToggle.isOn = true;
            IntegerValueAdjustInfo info = _optionsAdjustSettings.maxBalanceValueAdjustInfo;
            _optionsAdjustElements.MaxBalanceValue = info.GetFinalValue(_optionsAdjustElements.MaxBalanceValue, steps * info.valuePerStep);

            //Save Value
            PlayerPrefs.SetInt("maxBalanceToggle", 1);
            PlayerPrefs.SetInt("MaxBalanceValue", _optionsAdjustElements.MaxBalanceValue);
            PlayerPrefs.Save();
        }


        protected override void SetupCurrentAutoplayOptions () {
            _currentAutoplayOptions.fastSpin = _optionsAdjustElements.fastSpinningToggle.isOn;
            _currentAutoplayOptions.targetPlayRounds = _optionsAdjustElements.playRoundsToggle.isOn ? _optionsAdjustElements.PlayRoundsValue : -1;
            _currentAutoplayOptions.stoppingWonRate = _optionsAdjustElements.stoppingWonRateToggle.isOn ? _optionsAdjustElements.StoppingWonRateValue : -1;
            _currentAutoplayOptions.minBalance = _optionsAdjustElements.minBalanceToggle.isOn ? _optionsAdjustElements.MinBalanceValue : -1;
            _currentAutoplayOptions.maxBalance = _optionsAdjustElements.maxBalanceToggle.isOn ? _optionsAdjustElements.MaxBalanceValue : -1;
            _currentAutoplayOptions.stopWhenBonusOccurred = _optionsAdjustElements.stopWhenBonusOccurredToggle.isOn;
        }
        

        [Serializable]
        public class OptionsAdjustElements {
            public Toggle fastSpinningToggle;
            public Toggle playRoundsToggle;
            public Toggle stoppingWonRateToggle;
            public Toggle minBalanceToggle;
            public Toggle maxBalanceToggle;
            public Toggle stopWhenBonusOccurredToggle;

            public TextMeshProUGUI playRoundsValueText;
            public TextMeshProUGUI stoppingWonRateValueText;

            [SerializeField]
            public TMP_InputField minBalanceInputField;
            public TextMeshProUGUI minBalanceInputValue;

            public TMP_InputField maxBalanceInputField;
            public TextMeshProUGUI maxBalanceInputValue;

            [SerializeField] int _playRoundsValue = 10;
            public int PlayRoundsValue {
                get {
                    return _playRoundsValue;
                }
                set {
                    _playRoundsValue = value;
                    playRoundsValueText.text = _playRoundsValue.ToString("N0");
                }
            }

            [SerializeField] int _stoppingWonRateValue = 10;
            public int StoppingWonRateValue {
                get {
                    return _stoppingWonRateValue;
                }
                set {
                    _stoppingWonRateValue = value;
                    stoppingWonRateValueText.text = _stoppingWonRateValue.ToString("N0");
                }
            }

            [SerializeField] int _minBalanceValue = 1000;
            public int MinBalanceValue {
                get {
                    return _minBalanceValue;
                }
                set {
                    _minBalanceValue = value;
                    Debug.Log($"Start value: {_minBalanceValue}");
                    minBalanceInputField.text = _minBalanceValue.ToString("N0");
                }
            }

            [SerializeField] int _maxBalanceValue = 1000;
            public int MaxBalanceValue {
                get {
                    return _maxBalanceValue;
                }
                set {
                    _maxBalanceValue = value;
                    maxBalanceInputField.text = _maxBalanceValue.ToString("N0");
                }
            }

            public void onMinBalanceInputRecieve(string newValue)
            {
                minBalanceToggle.isOn = true;
                AutoplayOptionsPanelTypeA autoplayOptionsPanelTypeA = new AutoplayOptionsPanelTypeA();

                if (int.TryParse(newValue, out int result))
                {
                    MinBalanceValue = result;
                    minBalanceInputValue.text = result.ToString("N0");
                }

                PlayerPrefs.SetInt("MinBalanceValue", MinBalanceValue);
                PlayerPrefs.Save();
            }

            public void onMaxBalanceInputRecieve(string newValue)
            {
                maxBalanceToggle.isOn = true;
                AutoplayOptionsPanelTypeA autoplayOptionsPanelTypeA = new AutoplayOptionsPanelTypeA();

                if (int.TryParse(newValue, out int result))
                {
                    MaxBalanceValue = result;
                    maxBalanceInputValue.text = result.ToString("N0");
                }

                PlayerPrefs.SetInt("MaxBalanceValue", MaxBalanceValue);
                PlayerPrefs.Save();
            }

            public void CheckToggle()
            {
                if (fastSpinningToggle.isOn == true)
                    PlayerPrefs.SetInt("fastSpinningToggle", 1);
                else
                    PlayerPrefs.SetInt("fastSpinningToggle", 0);

                if (playRoundsToggle.isOn == true)
                    PlayerPrefs.SetInt("playRoundsToggle", 1);
                else
                    PlayerPrefs.SetInt("playRoundsToggle", 0);

                if (stoppingWonRateToggle.isOn == true)
                    PlayerPrefs.SetInt("stoppingWonRateToggle", 1);
                else
                    PlayerPrefs.SetInt("stoppingWonRateToggle", 0);

                if (minBalanceToggle.isOn == true)
                {
                    PlayerPrefs.SetInt("minBalanceToggle", 1);
                    PlayerPrefs.SetInt("isMinSet", 1);
                }
                else
                    PlayerPrefs.SetInt("minBalanceToggle", 0);

                if (maxBalanceToggle.isOn == true)
                {   
                    PlayerPrefs.SetInt("maxBalanceToggle", 1);
                    PlayerPrefs.SetInt("isMaxSet", 1);
                }
                else
                    PlayerPrefs.SetInt("maxBalanceToggle", 0);

                PlayerPrefs.Save();
            }
            public void RetriveSave()
            {
                //Fast Spin
                int fastSpinningToggled = PlayerPrefs.GetInt("fastSpinningToggle");
                if (fastSpinningToggled == 1)
                    fastSpinningToggle.isOn = true;
                else
                    fastSpinningToggle.isOn = false;

                //playRounds
                int playRoundsToggled = PlayerPrefs.GetInt("playRoundsToggle");
                int PlayRoundsValues = PlayerPrefs.GetInt("PlayRoundsValue", _playRoundsValue);
                PlayRoundsValue = PlayRoundsValues;
                if (playRoundsToggled == 1)
                    playRoundsToggle.isOn = true;
                else
                    playRoundsToggle.isOn = false;

                //stoppingWonRate
                int StoppingWonRateToggled = PlayerPrefs.GetInt("stoppingWonRateToggle");
                int StoppingWonRateValues = PlayerPrefs.GetInt("StoppingWonRateValue", _stoppingWonRateValue);
                StoppingWonRateValue = StoppingWonRateValues;
                if(StoppingWonRateToggled == 1)
                    stoppingWonRateToggle.isOn = true;
                else
                    stoppingWonRateToggle.isOn= false;

                //MinBalance
                int MinBalanceToggled = PlayerPrefs.GetInt("minBalanceToggle");
                int MinBalanceValues = PlayerPrefs.GetInt("MinBalanceValue", _minBalanceValue);
                int MinSetting = PlayerPrefs.GetInt("isMinSet");

                MinBalanceValue = MinBalanceValues;
                if (MinBalanceToggled == 1)
                    minBalanceToggle.isOn = true;
                else 
                    minBalanceToggle.isOn = false;

                //MaxBalance
                int MaxBalanceToggled = PlayerPrefs.GetInt("maxBalanceToggle");
                int MaxBalanceValues = PlayerPrefs.GetInt("MaxBalanceValue", _maxBalanceValue);
                int MaxSetting = PlayerPrefs.GetInt("isMaxSet");

                MaxBalanceValue = MaxBalanceValues;
                if (MaxBalanceToggled == 1)
                    maxBalanceToggle.isOn = true;
                else 
                    maxBalanceToggle.isOn = false;

                AutoplayOptionsPanelTypeA autoplayOptionsPanelTypeA = new AutoplayOptionsPanelTypeA();
                autoplayOptionsPanelTypeA.SetSettingStatus(MinSetting, MaxSetting);
            }
        }

        [Serializable]
        public struct IntegerValueAdjustInfo {
            public int valuePerStep;
            public int min;
            public int max;

            public int GetFinalValue (int current, int added = 0) {
                if (added > 0 && current + added < current) {
                    // overflowed
                    return max;
                }
                else if (added < 0 && current + added  > current) {
                    // negative overflowed
                    return min;
                }
                return MathTools.Clamp(current + added, min, max);
            }
        }


        [Serializable]
        class OptionsAdjustSettings {
            public IntegerValueAdjustInfo playRoundsValueAdjustInfo = new IntegerValueAdjustInfo {
                valuePerStep = 10,
                min = 10,
                max = 5000
            };
            public IntegerValueAdjustInfo stoppingWonRateValueAdjustInfo = new IntegerValueAdjustInfo {
                valuePerStep = 10,
                min = 10,
                max = 1000
            };
            public IntegerValueAdjustInfo minBalanceValueAdjustInfo = new IntegerValueAdjustInfo {
                valuePerStep = 1000,
                min = 1000,
                max = int.MaxValue
            };
            public IntegerValueAdjustInfo maxBalanceValueAdjustInfo = new IntegerValueAdjustInfo {
                valuePerStep = 1000,
                min = 1000,
                max = int.MaxValue
            };
            [Tooltip("Default offset from player's current balance.")]
            public int minBalanceValueOffsetFromPlayerCurrentBalance = -2000;
            [Tooltip("Default offset from player's current balance.")]
            public int maxBalanceValueOffsetFromPlayerCurrentBalance = 2000;
        }
    }
}
