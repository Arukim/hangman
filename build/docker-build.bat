cd ..\src
docker build -t arukim/hangman-web:%1 -f .\Hangman.WebUI\Dockerfile .
docker build -t arukim/hangman-orchestrator:%1 -f .\Hangman.Orchestrator\Dockerfile .
docker build -t arukim/hangman-dictionary:%1 -f .\Hangman.Dictionary\Dockerfile .
docker build -t arukim/hangman-processor:%1 -f .\Hangman.Processor\Dockerfile .