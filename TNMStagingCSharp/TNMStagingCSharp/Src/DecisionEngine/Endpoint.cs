// Copyright (C) 2017 Information Management Services, Inc.

using System;


namespace TNMStagingCSharp.Src.DecisionEngine
{
    public enum EndpointType
    {
        JUMP,
        VALUE,
        MATCH,
        STOP,
        ERROR
    }

    public interface IEndpoint
    {
        //========================================================================================================================
        // The type of endpoint
        // @return an EndPointType object
        //========================================================================================================================
        EndpointType getType();

        //========================================================================================================================
        // The value of the endpoint
        // @return a String value
        //========================================================================================================================
        String getValue();

        //========================================================================================================================
        // The key representing the field that the value is mapped to
        // @return a String field key
        //========================================================================================================================
        String getResultKey();

    }
}


