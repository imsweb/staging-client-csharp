/*
 * Copyright (C) 2022 Information Management Services, Inc.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using TNMStagingCSharp.Src.Staging.Entities;
using TNMStagingCSharp.Src.Staging.Entities.Impl;


namespace TNMStaging_UnitTestApp.Src.Staging.Entities
{
    [TestClass]
    public class EntitiesTest
    {

        [TestMethod]
        public void testEntities()
        {
            // ABH 9/13/2022: C# doesn't really have any functionality like this at present. Perhaps when we move to Visual Studio 2019. 
            // Basically we are testing that all of the classes are setup correctly with a constructor, getters and setters, an Equals function, 
            // a HashCode function, and specific functions.
            /*
            Assert.IsTrue(Error.class, allOf(hasValidBeanConstructor(), hasValidGettersAndSetters()));

            Assert.IsTrue(GlossaryDefinition.class, allOf(hasValidBeanConstructor(), hasValidGettersAndSetters(), hasValidBeanEquals(), hasValidBeanHashCode()));

            Assert.IsTrue(StagingColumnDefinition.class, allOf(hasValidBeanConstructor(), hasValidGettersAndSetters(), hasValidBeanEquals(), hasValidBeanHashCode()));
            Assert.IsTrue(StagingEndpoint.class, allOf(hasValidBeanConstructor(), hasValidGettersAndSetters()));
            Assert.IsTrue(StagingKeyMapping.class, allOf(hasValidBeanConstructor(), hasValidGettersAndSetters(), hasValidBeanEquals(), hasValidBeanHashCode()));
            Assert.IsTrue(StagingKeyValue.class, allOf(hasValidBeanConstructor(), hasValidGettersAndSetters(), hasValidBeanEquals(), hasValidBeanHashCode()));
            Assert.IsTrue(StagingMapping.class, allOf(hasValidBeanConstructor(), hasValidGettersAndSetters(), hasValidBeanEquals(), hasValidBeanHashCode()));
            Assert.IsTrue(StagingMetadata.class, allOf(hasValidBeanConstructor(), hasValidGettersAndSetters(), hasValidBeanEquals(), hasValidBeanHashCode()));
            Assert.IsTrue(StagingSchema.class, allOf(hasValidBeanConstructor(),
                    hasValidGettersAndSettersExcluding("inputMap", "outputMap"),
                    hasValidBeanEqualsExcluding("inputMap", "outputMap", "lastModified"),
                    hasValidBeanHashCodeExcluding("inputMap", "outputMap", "lastModified")));
            Assert.IsTrue(StagingSchemaInput.class, allOf(hasValidBeanConstructor(), hasValidGettersAndSetters(), hasValidBeanEquals(), hasValidBeanHashCode()));
            Assert.IsTrue(StagingSchemaOutput.class, allOf(hasValidBeanConstructor(), hasValidGettersAndSetters(), hasValidBeanEquals(), hasValidBeanHashCode()));
            Assert.IsTrue(StagingTable.class, allOf(hasValidBeanConstructor(),
                    hasValidGettersAndSetters(),
                    hasValidBeanEqualsExcluding("tableRows", "lastModified"),
                    hasValidBeanHashCodeExcluding("tableRows", "lastModified")));
            Assert.IsTrue(StagingTablePath.class, allOf(hasValidBeanConstructor(), hasValidGettersAndSetters(), hasValidBeanEquals(), hasValidBeanHashCode()));
            Assert.IsTrue(StagingTableRow.class, allOf(hasValidBeanConstructor(), hasValidGettersAndSettersFor("inputs", "endpoints")));
            */
        }
    }
}


