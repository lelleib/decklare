grammar DbgGrammar;
@parser::header {#pragma warning disable 3021}
@lexer::header {#pragma warning disable 3021}

program: definitionStatementList EOF;
statementList: NL? (statement (NL statement)*)? NL?;
definitionStatementList: NL? (definitionStatement (NL definitionStatement)*)? NL?;

statement: (varDefinition | assignment)? (command | expression);
definitionStatement: cardDefinition | commandDefinition | statement;

commandDefinition: LPAREN commandDeclaration RPAREN COLON block;

cardDefinition: cardName=ID LBRACKET NL? (propertyDefinition (NL propertyDefinition)*) NL? RBRACKET;

varDefinition: (varName=ID) COLON;
assignment: varRef COLONEQ;
command: expression* CW (CW | expression)*;
commandDeclaration: argumentDeclaration* CW (CW | argumentDeclaration)*;
propertyDefinition: varDefinition expression;

argumentDeclaration: LPAREN name=ID COLON typeDefinition RPAREN;
typeDefinition: mainType=CW (LT subType=typeDefinition GT)?;

expression: varRef | block | numericExpression | booleanExpression | numberPredicate | cardPredicate | enumLiteral | takeExpression | putExpression;
block: LCURLY statementList RCURLY;
varRef: varName=(ID | X | IT) varMemberPath;
varMemberPath: (SSUFFIX ID)*;
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

enumLiteral: variant=ID LPAREN enumType=CW RPAREN;

takeExpression: BACKSLASH command BACKSLASH;
putExpression: DIV command DIV;

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
BACKSLASH: '\\';

NUM: ('0' | [1-9][0-9]*);
X: 'X';
IT: 'IT';
CW: [A-Z][A-Z]+;
ID: ([a-z] | [a-zA-Z][a-zA-Z0-9_]+);
NL: ('\n' | '\r')+;
WS: (' ' | '\t') -> skip;
