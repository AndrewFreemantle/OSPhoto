#!/bin/sh
docker buildx build --platform linux/amd64 --push --tag "andrewfreemantle/os-photo:latest" .
