/*
 *
 *  * Copyright (c) 2013 no-pact.
 *  * All rights reserved.
 *  * no-pact PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 *
 */

namespace nopact.Commons.Utility
{
    public class FormatUtility {
	
        public static bool IsNullOrEmptyString(string value) {

            return (value == null || value.Trim().Equals(""));

        }

        public static string ReturnEmptyStringIfNull(string value) {

            return (value == null ? "" : value);

        }

        public static bool IsUsernameValid(string username, int minLength, int maxLength){

            // TODO check username format
            return IsLengthBetweenLimits(username, minLength, maxLength);

        }

        public static bool IsPasswordValid(string password, int minLength){

            return !password.Contains(" ")
                   && IsLengthEqualOrLongerThan(password, minLength);

        }
	
        public static bool IsLengthEqualOrLongerThan(string s, int minLength) {

            return s.Length >= minLength;

        }

        public static bool IsLengthEqualOrShorterThan(string s, int maxLength){

            return s.Length <= maxLength;

        }

        public static bool IsLengthBetweenLimits(string s, int minLength, int maxLength){

            return IsLengthEqualOrLongerThan(s, minLength) && IsLengthEqualOrShorterThan(s, maxLength);

        }
	
    }
}


