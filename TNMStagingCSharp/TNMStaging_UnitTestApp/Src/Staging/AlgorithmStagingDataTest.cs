using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using TNMStagingCSharp.Src.Staging.CS;
using TNMStagingCSharp.Src.Staging.Entities;
using TNMStagingCSharp.Src.Staging.EOD;
using TNMStagingCSharp.Src.Staging.TNM;

namespace TNMStaging_UnitTestApp.Src.Staging
{
    internal class AlgorithmStagingDataTest
    {
        private static readonly String SITE = "C509";
        private static readonly String HISTOLOGY = "8000";

        [TestMethod]
        void testCsConstructors()
        {
            CsStagingData empty = new CsStagingData();
            CsStagingData siteAndHistology = new CsStagingData(SITE, HISTOLOGY);
            CsStagingData withSsf25 = new CsStagingData(SITE, HISTOLOGY, "025");

            Assert.IsTrue(empty.getInput().Count == 0);
            AssertSiteAndHistology(siteAndHistology);
            AssertSiteAndHistology(withSsf25);
            Assert.AreEqual("025", withSsf25.getSsf(25));
        }

        private static void AssertSiteAndHistology(StagingData data)
        {
            Assert.AreEqual(SITE, data.getInput("site"));
            Assert.AreEqual(HISTOLOGY, data.getInput("hist"));
        }

        [TestMethod]
        void testTnmConstructors()
        {
            TnmStagingData empty = new TnmStagingData();
            TnmStagingData siteAndHistology = new TnmStagingData(SITE, HISTOLOGY);
            TnmStagingData withSsf25 = new TnmStagingData(SITE, HISTOLOGY, "025");

            Assert.IsTrue(empty.getInput().Count == 0);
            AssertSiteAndHistology(siteAndHistology);
            AssertSiteAndHistology(withSsf25);
            Assert.AreEqual("025", withSsf25.getSsf(25));
        }

        [TestMethod]
        void testEodConstructors()
        {
            EodStagingData empty = new EodStagingData();
            EodStagingData siteAndHistology = new EodStagingData(SITE, HISTOLOGY);
            EodStagingData withFirstDiscriminator = new EodStagingData(SITE, HISTOLOGY, "A");
            EodStagingData withBothDiscriminators = new EodStagingData(SITE, HISTOLOGY, "A", "B");

            Assert.IsTrue(empty.getInput().Count == 0);
            AssertSiteAndHistology(siteAndHistology);
            AssertSiteAndHistology(withFirstDiscriminator);
            Assert.AreEqual("A", withFirstDiscriminator.getInput(EodInput.DISCRIMINATOR_1));
            AssertSiteAndHistology(withBothDiscriminators);
            Assert.AreEqual("A", withBothDiscriminators.getInput(EodInput.DISCRIMINATOR_1));
            Assert.AreEqual("B", withBothDiscriminators.getInput(EodInput.DISCRIMINATOR_2));
        }

        [TestMethod]
        void testCsTypedInputAccess()
        {
            foreach (CsInput key in CsInput.Values)
            {
                CsStagingData data = new CsStagingData();
                data.setInput(key, "value");
                Assert.AreEqual("value", data.getInput(key));
                Assert.AreEqual("value", data.getInput(key.toString()));
            }
        }

        [TestMethod]
        void testTnmTypedInputAccess()
        {
            foreach (TnmInput key in TnmInput.Values)
            {
                TnmStagingData data = new TnmStagingData();
                data.setInput(key, "value");
                Assert.AreEqual("value", data.getInput(key));
                Assert.AreEqual("value", data.getInput(key.toString()));
            }
        }

        [TestMethod]
        void testEodTypedInputAccess()
        {
            foreach (EodInput key in EodInput.Values)
            {
                EodStagingData data = new EodStagingData();
                data.setInput(key, "value");
                Assert.AreEqual("value", data.getInput(key));
                Assert.AreEqual("value", data.getInput(key.toString()));
            }
        }

        [TestMethod]
        void testCsTypedOutputAccess()
        {
            foreach (CsOutput key in CsOutput.Values)
            {
                CsStagingData data = new CsStagingData();
                data.getOutput()[key.toString()] = "value";
                Assert.AreEqual("value", data.getOutput(key));
            }
        }

        [TestMethod]
        void testTnmTypedOutputAccess()
        {
            foreach (TnmOutput key in TnmOutput.Values)
            {
                TnmStagingData data = new TnmStagingData();
                data.getOutput()[key.toString()] = "value";
                Assert.AreEqual("value", data.getOutput(key));
            }
        }

        [TestMethod]
        void testEodTypedOutputAccess()
        {
            foreach (EodOutput key in EodOutput.Values)
            {
                EodStagingData data = new EodStagingData();
                data.getOutput()[key.toString()] = "value";
                Assert.AreEqual("value", data.getOutput(key));
            }
        }

        [TestMethod]
        void testCsSsfBoundaries()
        {
            for (int index = 1; index <= 25; index++)
            {
                CsStagingData data = new CsStagingData();
                data.setSsf(index, "value");
                Assert.AreEqual("value", data.getSsf(index));
            }
        }

        [TestMethod]
        void testTnmSsfBoundaries()
        {
            for (int index = 1; index <= 25; index++)
            {
                TnmStagingData data = new TnmStagingData();
                data.setSsf(index, "value");
                Assert.AreEqual("value", data.getSsf(index));
            }
        }

        [TestMethod]
        void testCsRejectsInvalidSsfIndexes()
        {
            int index = 0;
            for (int i = 0; i < 4; i++)
            {
                switch (i)
                {
                    case 0: 
                        index = -1;
                        break;
                    case 1:
                        index = 0;
                        break;
                    case 2:
                        index = 26;
                        break;
                    case 3:
                        index = int.MaxValue;
                        break;
                }
                CsStagingData data = new CsStagingData();

                bool exceptionThrown = false;
                try
                {
                    data.setSsf(index, "value");
                }
                catch (InvalidOperationException ex)
                {
                    exceptionThrown = true;
                }
                Assert.IsTrue(exceptionThrown);

                exceptionThrown = false;
                try
                {
                    data.getSsf(index);
                }
                catch (InvalidOperationException ex)
                {
                    exceptionThrown = true;
                }
                Assert.IsTrue(exceptionThrown);
            }
        }

        [TestMethod]
        void testTnmRejectsInvalidSsfIndexes()
        {
            int index = 0;
            for (int i = 0; i < 4; i++)
            {
                switch (i)
                {
                    case 0:
                        index = -1;
                        break;
                    case 1:
                        index = 0;
                        break;
                    case 2:
                        index = 26;
                        break;
                    case 3:
                        index = int.MaxValue;
                        break;
                }
                TnmStagingData data = new TnmStagingData();

                bool exceptionThrown = false;
                try
                {
                    data.setSsf(index, "value");
                }
                catch (InvalidOperationException ex)
                {
                    exceptionThrown = true;
                }
                Assert.IsTrue(exceptionThrown);

                exceptionThrown = false;
                try
                {
                    data.getSsf(index);
                }
                catch (InvalidOperationException ex)
                {
                    exceptionThrown = true;
                }
                Assert.IsTrue(exceptionThrown);
            }
        }

        [TestMethod]
        void testCsBuilder()
        {
            CsStagingData.CsStagingInputBuilder builder = new CsStagingData.CsStagingInputBuilder();
            builder.withInput(CsInput.BEHAVIOR, "3");
            builder.withSsf(1, "001");
            builder.withSsf(25, "025");

            CsStagingData data = builder.build();

            Assert.AreEqual("3", data.getInput(CsInput.BEHAVIOR));
            Assert.AreEqual("001", data.getSsf(1));
            Assert.AreEqual("025", data.getSsf(25));
        }

        [TestMethod]
        void testTnmBuilder()
        {
            TnmStagingData.TnmStagingInputBuilder builder = new TnmStagingData.TnmStagingInputBuilder();
            builder.withInput(TnmInput.BEHAVIOR, "3");
            builder.withSsf(1, "001");
            builder.withSsf(25, "025");

            TnmStagingData data = builder.build();

            Assert.AreEqual("3", data.getInput(TnmInput.BEHAVIOR));
            Assert.AreEqual("001", data.getSsf(1));
            Assert.AreEqual("025", data.getSsf(25));
        }

        [TestMethod]
        void testEodDiscriminatorBuilder()
        {
            EodStagingData.EodStagingInputBuilder builder = new EodStagingData.EodStagingInputBuilder();
            builder.withDisciminator1("A");
            builder.withDisciminator2("B");
            builder.withInput(EodInput.BEHAVIOR, "3");

            EodStagingData data = builder.build();

            Assert.AreEqual("A", data.getInput(EodInput.DISCRIMINATOR_1));
            Assert.AreEqual("B", data.getInput(EodInput.DISCRIMINATOR_2));
            Assert.AreEqual("3", data.getInput(EodInput.BEHAVIOR));
        }

        /*
        private static void AssertValidSsf(int index, Func<int, String> setter, Func<int, String> getter)
        {
            setter.accept(index, "value");
            Assert.AreEqual("value", getter.apply(index));
        }

        private static void AssertInvalidSsf(int index, BiConsumer<Integer, String> setter, Function<Integer, String> getter)
        {
            assertThrows(IllegalStateException.class, () -> setter.accept(index, "value")),
            assertThrows(IllegalStateException.class, () -> getter.apply(index)));
        }
        */
    }
}

/*
// Copyright (C) 2026 Information Management Services, Inc.
package com.imsweb.staging;

import java.util.function.BiConsumer;
import java.util.function.Function;

import org.junit.jupiter.api.Test;
import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.EnumSource;
import org.junit.jupiter.params.provider.ValueSource;

import com.imsweb.staging.cs.CsStagingData;
import com.imsweb.staging.cs.CsStagingData.CsInput;
import com.imsweb.staging.cs.CsStagingData.CsOutput;
import com.imsweb.staging.cs.CsStagingData.CsStagingInputBuilder;
import com.imsweb.staging.eod.EodStagingData;
import com.imsweb.staging.eod.EodStagingData.EodInput;
import com.imsweb.staging.eod.EodStagingData.EodOutput;
import com.imsweb.staging.eod.EodStagingData.EodStagingInputBuilder;
import com.imsweb.staging.tnm.TnmStagingData;
import com.imsweb.staging.tnm.TnmStagingData.TnmInput;
import com.imsweb.staging.tnm.TnmStagingData.TnmOutput;
import com.imsweb.staging.tnm.TnmStagingData.TnmStagingInputBuilder;

import static org.junit.jupiter.api.Assertions.assertAll;
import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.junit.jupiter.api.Assertions.assertTrue;

class AlgorithmStagingDataTest
{

    private static final String SITE = "C509";
    private static final String HISTOLOGY = "8000";

    [TestMethod]
    void testCsConstructors()
    {
        CsStagingData empty = new CsStagingData();
        CsStagingData siteAndHistology = new CsStagingData(SITE, HISTOLOGY);
        CsStagingData withSsf25 = new CsStagingData(SITE, HISTOLOGY, "025");

        assertAll(
                ()->Assert.IsTrue(empty.getInput().isEmpty()),
                ()->assertSiteAndHistology(siteAndHistology),
                ()->assertSiteAndHistology(withSsf25),
                ()->Assert.AreEqual("025", withSsf25.getSsf(25)));
    }

    [TestMethod]
    void testTnmConstructors()
    {
        TnmStagingData empty = new TnmStagingData();
        TnmStagingData siteAndHistology = new TnmStagingData(SITE, HISTOLOGY);
        TnmStagingData withSsf25 = new TnmStagingData(SITE, HISTOLOGY, "025");

        assertAll(
                ()->Assert.IsTrue(empty.getInput().isEmpty()),
                ()->assertSiteAndHistology(siteAndHistology),
                ()->assertSiteAndHistology(withSsf25),
                ()->Assert.AreEqual("025", withSsf25.getSsf(25)));
    }

    [TestMethod]
    void testEodConstructors()
    {
        EodStagingData empty = new EodStagingData();
        EodStagingData siteAndHistology = new EodStagingData(SITE, HISTOLOGY);
        EodStagingData withFirstDiscriminator = new EodStagingData(SITE, HISTOLOGY, "A");
        EodStagingData withBothDiscriminators = new EodStagingData(SITE, HISTOLOGY, "A", "B");

        assertAll(
                ()->Assert.IsTrue(empty.getInput().isEmpty()),
                ()->assertSiteAndHistology(siteAndHistology),
                ()->assertSiteAndHistology(withFirstDiscriminator),
                ()->Assert.AreEqual("A", withFirstDiscriminator.getInput(EodInput.DISCRIMINATOR_1)),
                ()->assertSiteAndHistology(withBothDiscriminators),
                ()->Assert.AreEqual("A", withBothDiscriminators.getInput(EodInput.DISCRIMINATOR_1)),
                ()->Assert.AreEqual("B", withBothDiscriminators.getInput(EodInput.DISCRIMINATOR_2)));
    }

    @ParameterizedTest
    @EnumSource(CsInput.class)
    void testCsTypedInputAccess(CsInput key)
    {
        CsStagingData data = new CsStagingData();

        data.setInput(key, "value");

        assertAll(
                ()->Assert.AreEqual("value", data.getInput(key)),
                ()->Assert.AreEqual("value", data.getInput(key.toString())));
    }

    @ParameterizedTest
    @EnumSource(TnmInput.class)
    void testTnmTypedInputAccess(TnmInput key)
    {
        TnmStagingData data = new TnmStagingData();

        data.setInput(key, "value");

        assertAll(
                ()->Assert.AreEqual("value", data.getInput(key)),
                ()->Assert.AreEqual("value", data.getInput(key.toString())));
    }

    @ParameterizedTest
    @EnumSource(EodInput.class)
    void testEodTypedInputAccess(EodInput key)
    {
        EodStagingData data = new EodStagingData();

        data.setInput(key, "value");

        assertAll(
                ()->Assert.AreEqual("value", data.getInput(key)),
                ()->Assert.AreEqual("value", data.getInput(key.toString())));
    }

    @ParameterizedTest
    @EnumSource(CsOutput.class)
    void testCsTypedOutputAccess(CsOutput key)
    {
        CsStagingData data = new CsStagingData();
        data.getOutput().put(key.toString(), "value");

        Assert.AreEqual("value", data.getOutput(key));
    }

    @ParameterizedTest
    @EnumSource(TnmOutput.class)
    void testTnmTypedOutputAccess(TnmOutput key)
    {
        TnmStagingData data = new TnmStagingData();
        data.getOutput().put(key.toString(), "value");

        Assert.AreEqual("value", data.getOutput(key));
    }

    @ParameterizedTest
    @EnumSource(EodOutput.class)
    void testEodTypedOutputAccess(EodOutput key)
    {
        EodStagingData data = new EodStagingData();
        data.getOutput().put(key.toString(), "value");

        Assert.AreEqual("value", data.getOutput(key));
    }

    @ParameterizedTest
    @ValueSource(ints = { 1, 25})
    void testCsSsfBoundaries(int index)
    {
        CsStagingData data = new CsStagingData();

        assertValidSsf(index, data::setSsf, data::getSsf);
    }

    @ParameterizedTest
    @ValueSource(ints = { 1, 25})
    void testTnmSsfBoundaries(int index)
    {
        TnmStagingData data = new TnmStagingData();

        assertValidSsf(index, data::setSsf, data::getSsf);
    }

    @ParameterizedTest
    @ValueSource(ints = { -1, 0, 26, Integer.MAX_VALUE})
    void testCsRejectsInvalidSsfIndexes(int index)
    {
        CsStagingData data = new CsStagingData();

        assertInvalidSsf(index, data::setSsf, data::getSsf);
    }

    @ParameterizedTest
    @ValueSource(ints = { -1, 0, 26, Integer.MAX_VALUE})
    void testTnmRejectsInvalidSsfIndexes(int index)
    {
        TnmStagingData data = new TnmStagingData();

        assertInvalidSsf(index, data::setSsf, data::getSsf);
    }

    [TestMethod]
    void testCsBuilder()
    {
        CsStagingData data = new CsStagingInputBuilder()
                .withInput(CsInput.BEHAVIOR, "3")
                .withSsf(1, "001")
                .withSsf(25, "025")
                .build();

        assertAll(
                ()->Assert.AreEqual("3", data.getInput(CsInput.BEHAVIOR)),
                ()->Assert.AreEqual("001", data.getSsf(1)),
                ()->Assert.AreEqual("025", data.getSsf(25)));
    }

    [TestMethod]
    void testTnmBuilder()
    {
        TnmStagingData data = new TnmStagingInputBuilder()
                .withInput(TnmInput.BEHAVIOR, "3")
                .withSsf(1, "001")
                .withSsf(25, "025")
                .build();

        assertAll(
                ()->Assert.AreEqual("3", data.getInput(TnmInput.BEHAVIOR)),
                ()->Assert.AreEqual("001", data.getSsf(1)),
                ()->Assert.AreEqual("025", data.getSsf(25)));
    }

    [TestMethod]
    void testEodDiscriminatorBuilder()
    {
        EodStagingData data = new EodStagingInputBuilder()
                .withDisciminator1("A")
                .withDisciminator2("B")
                .withInput(EodInput.BEHAVIOR, "3")
                .build();

        assertAll(
                ()->Assert.AreEqual("A", data.getInput(EodInput.DISCRIMINATOR_1)),
                ()->Assert.AreEqual("B", data.getInput(EodInput.DISCRIMINATOR_2)),
                ()->Assert.AreEqual("3", data.getInput(EodInput.BEHAVIOR)));
    }

    private static void assertSiteAndHistology(com.imsweb.staging.entities.StagingData data)
    {
        assertAll(
                ()->Assert.AreEqual(SITE, data.getInput("site")),
                ()->Assert.AreEqual(HISTOLOGY, data.getInput("hist")));
    }

    private static void assertValidSsf(int index, BiConsumer<Integer, String> setter, Function<Integer, String> getter)
    {
        setter.accept(index, "value");

        Assert.AreEqual("value", getter.apply(index));
    }

    private static void assertInvalidSsf(int index, BiConsumer<Integer, String> setter, Function<Integer, String> getter)
    {
        assertAll(
                ()->assertThrows(IllegalStateException.class, () -> setter.accept(index, "value")),
                () -> assertThrows(IllegalStateException.class, () -> getter.apply(index)));
    }
}

*/