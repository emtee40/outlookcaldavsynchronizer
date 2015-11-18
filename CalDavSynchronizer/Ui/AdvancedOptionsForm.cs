﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CalDavSynchronizer.Contracts;
using CalDavSynchronizer.Utilities;
using log4net;

namespace CalDavSynchronizer.Ui
{
  public partial class AdvancedOptionsForm : Form
  {
    private static readonly ILog s_logger = LogManager.GetLogger (System.Reflection.MethodInfo.GetCurrentMethod().DeclaringType);
    private MappingConfigurationBase _mappingConfiguration;
    private readonly Func<MappingConfigurationBase, MappingConfigurationBase> _coerceMappingConfiguration;
    private readonly IConfigurationFormFactory _configurationFormFactory;

    public AdvancedOptionsForm (
        Func<MappingConfigurationBase, MappingConfigurationBase> coerceMappingConfiguration,
        IConfigurationFormFactory configurationFormFactory)
    {
      _configurationFormFactory = configurationFormFactory;
      _coerceMappingConfiguration = coerceMappingConfiguration;
      InitializeComponent();
    }

    public AdvancedOptionsForm (Func<MappingConfigurationBase, MappingConfigurationBase> coerceMappingConfiguration)
        : this (coerceMappingConfiguration, ConfigurationFormFactory.Instance)
    {
    }

    public AdvancedOptions Options
    {
      get
      {
        return new AdvancedOptions (
            _closeConnectionAfterEachRequestCheckBox.Checked,
            new ProxyOptions()
            {
                ProxyUseDefault = _useSystemProxyCheckBox.Checked,
                ProxyUseManual = _useManualProxyCheckBox.Checked,
                ProxyUrl = _proxyUrlTextBox.Text,
                ProxyUserName = _userNameTextBox.Text,
                ProxyPassword = _passwordTextBox.Text
            },
            _mappingConfiguration);
      }
      set
      {
        _closeConnectionAfterEachRequestCheckBox.Checked = value.CloseConnectionAfterEachRequest;

        _useSystemProxyCheckBox.Checked = value.ProxyOptions.ProxyUseDefault;
        _useManualProxyCheckBox.Checked = value.ProxyOptions.ProxyUseManual;
        _proxyUrlTextBox.Text = value.ProxyOptions.ProxyUrl;
        _userNameTextBox.Text = value.ProxyOptions.ProxyUserName;
        _passwordTextBox.Text = value.ProxyOptions.ProxyPassword;
        _manualProxyGroupBox.Enabled = value.ProxyOptions.ProxyUseManual;

        _mappingConfiguration = value.MappingConfiguration;
      }
    }

    private void OkButton_Click (object sender, EventArgs e)
    {
      StringBuilder errorMessageBuilder = new StringBuilder();
      if (_useManualProxyCheckBox.Checked && !ValidateProxyUrl (errorMessageBuilder))
      {
        MessageBox.Show (errorMessageBuilder.ToString(), "The Proxy Url is invalid", MessageBoxButtons.OK, MessageBoxIcon.Error);
        DialogResult = DialogResult.None;
      }
      else
      {
        DialogResult = DialogResult.OK;
      }
    }

    private void _useManualProxyCheckBox_CheckedChanged (object sender, EventArgs e)
    {
      if (_useManualProxyCheckBox.Checked)
        _useSystemProxyCheckBox.Checked = false;
      _manualProxyGroupBox.Enabled = _useManualProxyCheckBox.Checked;
    }

    private void _useSystemProxyCheckBox_CheckedChanged (object sender, EventArgs e)
    {
      if (_useSystemProxyCheckBox.Checked)
        _useManualProxyCheckBox.Checked = false;
    }

    private bool ValidateProxyUrl (StringBuilder errorMessageBuilder)
    {
      bool result = true;

      if (string.IsNullOrWhiteSpace (_proxyUrlTextBox.Text))
      {
        errorMessageBuilder.AppendLine ("- The Proxy Url is empty.");
        return false;
      }

      if (_proxyUrlTextBox.Text.Trim() != _proxyUrlTextBox.Text)
      {
        errorMessageBuilder.AppendLine ("- The Proxy Url cannot end/start with whitespaces.");
        result = false;
      }

      try
      {
        new Uri (_proxyUrlTextBox.Text).ToString();
      }
      catch (Exception x)
      {
        errorMessageBuilder.AppendFormat ("- The Proxy Url is not a well formed Url. ({0})", x.Message);
        errorMessageBuilder.AppendLine();
        result = false;
      }

      return result;
    }

    private void _mappingConfigurationButton_Click (object sender, EventArgs e)
    {
      try
      {
        var mappingConfiguration = _coerceMappingConfiguration (_mappingConfiguration);
        if (mappingConfiguration != null)
        {
          var configurationForm = mappingConfiguration.CreateConfigurationForm (_configurationFormFactory);
          if (configurationForm.Display())
            _mappingConfiguration = configurationForm.Options;
        }
        else
        {
          MessageBox.Show ("Mapping configuration not available.");
          return;
        }
      }
      catch (Exception x)
      {
        ExceptionHandler.Instance.HandleException (x, s_logger);
      }
    }
  }
}