CC="gmcs"

all:
	$(CC) -debug+ -out:Tools.dll -target:library -define:GENTIME dfa.cs lexer.cs parser.cs olist.cs serialise.cs genbase0.cs
	$(CC) -debug+ -r:Tools.dll lg.cs
	mono lg.exe pg.lexer
	$(CC) -debug+ -r:Tools.dll pg.cs pg.lexer.cs
	mono lg.exe cs0.lexer
	mono pg.exe cs0.parser
	$(CC) -debug+ -out:Tools.dll -target:library -define:GENTIME dfa.cs lexer.cs parser.cs olist.cs serialise.cs genbase.cs cs0.lexer.cs cs0.parser.cs
	$(CC) -debug+ -r:Tools.dll lg.cs
	$(CC) -debug+ -r:Tools.dll pg.cs pg.lexer.cs 

clean:
	rm Tools.dll* pg.exe* lg.exe* pg.lexer.cs cs0.lexer.cs cs0.parser.cs
