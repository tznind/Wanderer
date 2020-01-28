grammar Wander;

/*
 * Parser Rules
 */

methodcall           : word LBRACE arguments RBRACE;
arguments            : (arg COMMA?)*;
arg                  : WORD;


/*
 * Lexer Rules
 */


fragment LOWERCASE  : [a-z] ;
fragment UPPERCASE  : [A-Z] ;
fragment DIGIT      : '0'..'9' ;

COMMA               : ',';

word                : (LOWERCASE | UPPERCASE)+ ;
TEXT                : '"' .*? '"' ;
WHITESPACE          : (' '|'\t')+ -> skip ;
NEWLINE             : ('\r'? '\n' | '\r')+ ;