#!/bin/sh
svcutil --out=DomainServices.cs --config=App.config ${PWD}/../Services/bin/Debug/Warehouse.Services.dll
