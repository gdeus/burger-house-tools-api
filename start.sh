#!/bin/bash

echo "================================================="
echo "Iniciando o ambiente - Burger House Tools API..."
echo "================================================="

echo "[1/4] Subindo containers no Docker..."
docker compose up -d

echo "Aguardando 15 segundos para inicialização do banco de dados..."
sleep 15

cd burger-house-tools-api || exit

echo "[2/4] Restaurando pacotes e atualizando o banco de dados..."
dotnet restore

dotnet tool install --global dotnet-ef 2>/dev/null || true

dotnet ef database update

echo "[3/4] Iniciando a API..."
dotnet run