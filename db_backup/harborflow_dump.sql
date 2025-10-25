--
-- PostgreSQL database dump
--

\restrict 5eNPBs21qkM2Lc8z3Bi1AtjANlrruL1VNHvi5IZPsnnutvGYiLJnECDwQa5ELcP

-- Dumped from database version 17.6
-- Dumped by pg_dump version 17.6 (Homebrew)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: ApprovalHistories; Type: TABLE; Schema: public; Owner: marseillosatrian
--

CREATE TABLE public."ApprovalHistories" (
    "ApprovalId" uuid NOT NULL,
    "RequestId" uuid NOT NULL,
    "ApprovedBy" uuid NOT NULL,
    "Action" integer NOT NULL,
    "ActionDate" timestamp with time zone NOT NULL,
    "Reason" text NOT NULL
);


ALTER TABLE public."ApprovalHistories" OWNER TO marseillosatrian;

--
-- Name: ServiceRequests; Type: TABLE; Schema: public; Owner: marseillosatrian
--

CREATE TABLE public."ServiceRequests" (
    "RequestId" uuid NOT NULL,
    "VesselImo" text NOT NULL,
    "RequestedBy" uuid NOT NULL,
    "ServiceType" integer NOT NULL,
    "Status" integer NOT NULL,
    "RequestDate" timestamp with time zone NOT NULL,
    "RequiredDate" timestamp with time zone,
    "Description" text NOT NULL,
    "Documents" text[] NOT NULL,
    "Notes" text NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL
);


ALTER TABLE public."ServiceRequests" OWNER TO marseillosatrian;

--
-- Name: Users; Type: TABLE; Schema: public; Owner: marseillosatrian
--

CREATE TABLE public."Users" (
    "UserId" uuid NOT NULL,
    "Username" text NOT NULL,
    "PasswordHash" text NOT NULL,
    "Email" text NOT NULL,
    "FullName" text NOT NULL,
    "Role" integer NOT NULL,
    "Organization" text NOT NULL,
    "IsActive" boolean NOT NULL,
    "LastLogin" timestamp with time zone,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL
);


ALTER TABLE public."Users" OWNER TO marseillosatrian;

--
-- Name: VesselPositions; Type: TABLE; Schema: public; Owner: marseillosatrian
--

CREATE TABLE public."VesselPositions" (
    "Id" uuid NOT NULL,
    "VesselImo" character varying(7) NOT NULL,
    "Latitude" numeric NOT NULL,
    "Longitude" numeric NOT NULL,
    "PositionTimestamp" timestamp with time zone NOT NULL,
    "SpeedOverGround" numeric NOT NULL,
    "CourseOverGround" numeric NOT NULL,
    "Source" integer NOT NULL,
    "Accuracy" numeric NOT NULL
);


ALTER TABLE public."VesselPositions" OWNER TO marseillosatrian;

--
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: public; Owner: marseillosatrian
--

CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


ALTER TABLE public."__EFMigrationsHistory" OWNER TO marseillosatrian;

--
-- Name: vessels; Type: TABLE; Schema: public; Owner: marseillosatrian
--

CREATE TABLE public.vessels (
    "IMO" character varying(7) NOT NULL,
    "Mmsi" character varying(9) NOT NULL,
    "Name" text NOT NULL,
    "VesselType" integer NOT NULL,
    "FlagState" text NOT NULL,
    "LengthOverall" numeric NOT NULL,
    "Beam" numeric NOT NULL,
    "GrossTonnage" numeric NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "Metadata" jsonb NOT NULL
);


ALTER TABLE public.vessels OWNER TO marseillosatrian;

--
-- Data for Name: ApprovalHistories; Type: TABLE DATA; Schema: public; Owner: marseillosatrian
--

COPY public."ApprovalHistories" ("ApprovalId", "RequestId", "ApprovedBy", "Action", "ActionDate", "Reason") FROM stdin;
\.


--
-- Data for Name: ServiceRequests; Type: TABLE DATA; Schema: public; Owner: marseillosatrian
--

COPY public."ServiceRequests" ("RequestId", "VesselImo", "RequestedBy", "ServiceType", "Status", "RequestDate", "RequiredDate", "Description", "Documents", "Notes", "CreatedAt", "UpdatedAt") FROM stdin;
\.


--
-- Data for Name: Users; Type: TABLE DATA; Schema: public; Owner: marseillosatrian
--

COPY public."Users" ("UserId", "Username", "PasswordHash", "Email", "FullName", "Role", "Organization", "IsActive", "LastLogin", "CreatedAt", "UpdatedAt") FROM stdin;
\.


--
-- Data for Name: VesselPositions; Type: TABLE DATA; Schema: public; Owner: marseillosatrian
--

COPY public."VesselPositions" ("Id", "VesselImo", "Latitude", "Longitude", "PositionTimestamp", "SpeedOverGround", "CourseOverGround", "Source", "Accuracy") FROM stdin;
\.


--
-- Data for Name: __EFMigrationsHistory; Type: TABLE DATA; Schema: public; Owner: marseillosatrian
--

COPY public."__EFMigrationsHistory" ("MigrationId", "ProductVersion") FROM stdin;
20251023031246_InitialUserSetup	9.0.0
\.


--
-- Data for Name: vessels; Type: TABLE DATA; Schema: public; Owner: marseillosatrian
--

COPY public.vessels ("IMO", "Mmsi", "Name", "VesselType", "FlagState", "LengthOverall", "Beam", "GrossTonnage", "CreatedAt", "UpdatedAt", "Metadata") FROM stdin;
\.


--
-- Name: ApprovalHistories PK_ApprovalHistories; Type: CONSTRAINT; Schema: public; Owner: marseillosatrian
--

ALTER TABLE ONLY public."ApprovalHistories"
    ADD CONSTRAINT "PK_ApprovalHistories" PRIMARY KEY ("ApprovalId");


--
-- Name: ServiceRequests PK_ServiceRequests; Type: CONSTRAINT; Schema: public; Owner: marseillosatrian
--

ALTER TABLE ONLY public."ServiceRequests"
    ADD CONSTRAINT "PK_ServiceRequests" PRIMARY KEY ("RequestId");


--
-- Name: Users PK_Users; Type: CONSTRAINT; Schema: public; Owner: marseillosatrian
--

ALTER TABLE ONLY public."Users"
    ADD CONSTRAINT "PK_Users" PRIMARY KEY ("UserId");


--
-- Name: VesselPositions PK_VesselPositions; Type: CONSTRAINT; Schema: public; Owner: marseillosatrian
--

ALTER TABLE ONLY public."VesselPositions"
    ADD CONSTRAINT "PK_VesselPositions" PRIMARY KEY ("Id");


--
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: public; Owner: marseillosatrian
--

ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- Name: vessels PK_vessels; Type: CONSTRAINT; Schema: public; Owner: marseillosatrian
--

ALTER TABLE ONLY public.vessels
    ADD CONSTRAINT "PK_vessels" PRIMARY KEY ("IMO");


--
-- Name: IX_VesselPositions_VesselImo; Type: INDEX; Schema: public; Owner: marseillosatrian
--

CREATE INDEX "IX_VesselPositions_VesselImo" ON public."VesselPositions" USING btree ("VesselImo");


--
-- Name: IX_vessels_Mmsi; Type: INDEX; Schema: public; Owner: marseillosatrian
--

CREATE UNIQUE INDEX "IX_vessels_Mmsi" ON public.vessels USING btree ("Mmsi");


--
-- Name: VesselPositions FK_VesselPositions_vessels_VesselImo; Type: FK CONSTRAINT; Schema: public; Owner: marseillosatrian
--

ALTER TABLE ONLY public."VesselPositions"
    ADD CONSTRAINT "FK_VesselPositions_vessels_VesselImo" FOREIGN KEY ("VesselImo") REFERENCES public.vessels("IMO") ON DELETE CASCADE;


--
-- PostgreSQL database dump complete
--

\unrestrict 5eNPBs21qkM2Lc8z3Bi1AtjANlrruL1VNHvi5IZPsnnutvGYiLJnECDwQa5ELcP

