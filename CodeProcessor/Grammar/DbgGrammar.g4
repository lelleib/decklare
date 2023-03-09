grammar DbgGrammar;
@parser::header {#pragma warning disable 3021}
@lexer::header {#pragma warning disable 3021}

program: definitionStatementList EOF;
statementList: NL? (statement (NL statement)*)? NL?;
definitionStatementList: NL? ((commandDefinition | statement) (NL (commandDefinition | statement))*)? NL?;

statement: (varDefinition | assignment)? (command | expression);

commandDefinition: LPAREN commandDeclaration RPAREN COLON block;

varDefinition: (varName=ID) COLON;
assignment: varRef COLONEQ;
command: CW (CW | expression)*;
commandDeclaration: CW (CW | argumentDeclaration)*;
argumentDeclaration: LPAREN name=ID COLON typeDefinition RPAREN;
typeDefinition: mainType=CW (LT subType=typeDefinition GT)?;

expression: varRef | block | numericExpression | booleanExpression | numberPredicate | cardPredicate | enumLiteral;
block: LCURLY statementList RCURLY;
//varRef: ID varRef2;
//varRef2: (SSUFFIX varRef varRef2)?;
varRef: ID (SSUFFIX ID)*;
//numericExpression: (NUM | varRef) numericExpression2;
//numericExpression2: (op=(MULT | DIV | PLUS | MINUS) numericExpression numericExpression2)?;
//booleanExpression: (relationExpression | enumIsExpression) booleanExpression2;
//booleanExpression2: (op=(AND | OR) booleanExpression booleanExpression2)?;
numericExpression: numericExpression (MULT | DIV | MOD) numericExpression
                    | numericExpression (PLUS | MINUS) numericExpression
                    | (NUM | varRef)
                    | LPAREN numericExpression RPAREN;
booleanExpression: booleanExpression AND booleanExpression
                    | booleanExpression OR booleanExpression
                    | (relationExpression | enumIsExpression | listHasExpression)
                    | varRef
                    | LPAREN booleanExpression RPAREN;
relationExpression: numericExpression op=(LT | GT | LTE | GTE | EQ | NEQ) numericExpression;
enumIsExpression: varRef IS NOT? varRef;
listHasExpression: varRef HAS NO? varRef;

numberPredicate: LT booleanExpression GT;
cardPredicate: LBRACKET booleanExpression RBRACKET;

enumLiteral: variant=CW COLON enumType=CW;

PLUS: '+';
MINUS: '-';
MULT: '*';
DIV: '/';
MOD: '%';

LT: '<';
GT: '>';
LTE: '<=';
GTE: '>=';
EQ: '=';
NEQ: '!=';

AND: '&';
OR: '|';

IS: 'IS';
NOT: 'NOT';
HAS: 'HAS';
NO: 'NO';

COLONEQ: ':=';
COLON: ':';
SSUFFIX: '\'s';

LCURLY: '{';
RCURLY: '}';
LPAREN: '(';
RPAREN: ')';
LBRACKET: '[';
RBRACKET: ']';

/*CARD: 'CARD';
PILE: 'PILE';
SUPPLY: 'SUPPLY';
NUMBER: 'NUMBER';
ENUM: 'ENUM';
PLAYER: 'PLAYER';
LIST: 'LIST';*/

NUM: ('0' | [1-9][0-9]*);
CW: [A-Z][A-Z]+;
ID: [a-zA-Z][a-zA-Z0-9_]*;
NL: ('\n' | '\r')+;
WS: (' ' | '\t') -> skip;
