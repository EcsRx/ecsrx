set version=0.2.0
dotnet pack ../src/EcsRx -c Release -o ../../_dist /p:version=%version%
dotnet pack ../src/EcsRx.Systems -c Release -o ../../_dist /p:version=%version%
dotnet pack ../src/EcsRx.Views -c Release -o ../../_dist /p:version=%version%
dotnet pack ../src/EcsRx.Infrastructure -c Release -o ../../_dist /p:version=%version%