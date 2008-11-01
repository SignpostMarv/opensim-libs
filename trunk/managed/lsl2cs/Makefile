CC="gmcs"

all: output.exe Tools.dll

Tools.dll: Tools/lg.cs
	(cd Tools; make)
	cp Tools/Tools.dll Tools/lg.exe Tools/pg.exe .

output.exe: CSCodeGenerator.cs LSL2CSCodeTransformer.cs CSReservedWords.cs lsl.lexer.cs lsl.parser.cs output.cs
	$(CC) -debug+ -r:Tools.dll output.cs lsl.lexer.cs lsl.parser.cs CSCodeGenerator.cs LSL2CSCodeTransformer.cs CSReservedWords.cs

lsl.lexer.cs: lsl.lexer Tools.dll
	mono lg.exe lsl.lexer

lsl.parser.cs: lsl.lexer lsl.parser Tools.dll
	mono pg.exe -C lsl.parser

clean:
	(cd Tools; make clean)
	rm lg.exe pg.exe Tools.dll lsl.lexer.cs lsl.parser.cs output.exe*
