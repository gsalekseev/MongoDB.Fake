﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace MongoDB.Fake.Tests.Filters.Cases.Not
{
    internal abstract class NotTestCaseBase : TestCaseBase, IFilterTestCase<NotFilterTests, SimpleTestDocument>
    {
        public override IEnumerable<SimpleTestDocument> GetExpectedResult()
        {
            var expectedResultIds = new[]
            {
                new Guid("00000000-0000-0000-0000-000000000001"),
                new Guid("00000000-0000-0000-0000-000000000002")
            };
            return GetTestData().Where(d => expectedResultIds.Contains(d.Id));
        }
    }
}