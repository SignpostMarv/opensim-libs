CC="gmcs"

all: lsl.parser.cs

Tools.dll: Tools/lg.cs
	(cd Tools; make)
	cp Tools/Tools.dll Tools/lg.exe Tools/pg.exe .

lsl.lexer.cs: Tools.dll
	mono lg.exe lsl.lexer

lsl.parser.cs: lsl.lexer.cs
	mono pg.exe -C lsl.parser

clean:
	(cd Tools; make clean)
	rm lg.exe pg.exe Tools.dll lsl.lexer.cs lsl.parser.cs
