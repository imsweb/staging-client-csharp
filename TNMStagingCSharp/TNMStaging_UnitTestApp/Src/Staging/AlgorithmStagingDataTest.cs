using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNMStaging_UnitTestApp.Src.Staging
{
    internal class AlgorithmStagingDataTest
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