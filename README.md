## Opis jak uruchomić projekt

### Prerequisites

- dotnet core w wersji [7.0](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-7.0.408-windows-x64-installer)
- [MSSQL Server](https://www.microsoft.com/pl-pl/sql-server/sql-server-downloads) **wybierz na stronie wersje bezpłatną oznaczoną jako Developer**
- vscode (zakładam że każdy ma)
- git (zakładam ze każdy ma)

### Aby uruchomić

Musisz mieć uruchomioną usługę po instalacji MSSQL (automatycznie się uruchamia na restarcie po instalacji)

1. `git clone https://github.com/wjakub072/WorkoutPlanner.git`
2. Otwierasz utworzony katalog w vs code
3. _Ctrl+Shift+P_ a nastepnie wybierasz _Tasks: Run Task_
4. Z listy wybierz taska o nazwie _seed_
5. Powinien ci się otworzyć terminal w vs code w którym musisz wpisać pierwszego użytkownika aplikacji, będzie on miał role admina, jego loginem musi być mail, osobiście polecam *admin@o2.pl* i wybrane przez siebie hasło
6. Czekasz na informacje _"Done seeding database."_
7. _Ctrl+Shift+P_ a nastepnie wybierasz _Tasks: Run Task_
8. Wybierasz z listy taska _run_
9. Wchodzisz w otwarty terminal i weryfikujesz swój port, będzie on zawarty we wiadomości _Now listening on: http://localhost:5290_
10. Program jest uruchomiony i czeka na requesty

### Do testowania requestów daje link do kolekcji w postmanie, jednak nie jestem pewien czy wam zadziała

https://interstellar-escape-906687.postman.co/workspace/ArtSphere~2ca75260-64e6-4627-864e-2876997b2eb6/collection/26035395-0d58a731-035c-4922-9a76-c8976c0e75c7?action=share&creator=26035395&active-environment=26035395-b373ad94-0c46-404c-8c6f-ac001a9aeaad

### W katalogu client znajduje się serwis apiService jest to prosty klient do tego api z gotową autoryzacją

Dodaje do niego jeszcze przykładowy AuthService abyście mieli informacje jak korzystać z tego apiService. ApiService.js nalezy jeszcze zmienić ponieważ korzysta z pinii, a nie wiem czy front będzie ją implementował. Wystarczy żebyście gdziekolwiek przechowywali ten token i nastepnie dostarczali go do requestów tak jak tam jest to napisane.
