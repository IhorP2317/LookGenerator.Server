﻿namespace LookGenerator.Application.Common.Exceptions ;

    public class AccessForbiddenException:Exception
    {
        public AccessForbiddenException()
            : base("Access denied!") { }

        public AccessForbiddenException(string message)
            : base(message) { }
    }