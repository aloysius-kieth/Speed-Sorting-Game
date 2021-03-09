using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CredentialsValidator : MonoBehaviour {

    const int MAX_MOBILE_LENGTH = 8;

    public static bool validateName(string _name) {
        if (!string.IsNullOrEmpty(_name)) {
            return true;
        }
        return false;
    }
    public static bool validateMobile(string _mobile) {
        if (!string.IsNullOrEmpty(_mobile) && _mobile.Length == MAX_MOBILE_LENGTH) {
            return true;
        }
        return false;
    }
    public static bool validateEmail(string _email) {
        if (!string.IsNullOrEmpty(_email)) {
            int countSymbol = _email.Length - _email.Replace("@", "").Length;
            if (countSymbol == 1 && _email.Contains(".") && _email.IndexOf('@') != 0 && !_email.Contains(" ")) {
                return true;
            }
        }
        return false;
        
    }
}
