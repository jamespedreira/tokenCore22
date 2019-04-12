-- INCLUI O USUÁRIO PARA ACESSAR O TMS
INSERT INTO TNU_USUARI
  (USU_IDENTI,
   USU_DESCRI,
   USU_LOGIN,
   USU_SENHA,
   USU_EMAIL,
   USU_CTRSES,
   USU_MAXFRM,
   USU_SENEXP,
   USU_SENALT,
   USU_ATIVO,
   USU_ALTSEN,
   USU_MAILEN,
   USU_MASTER,
   USU_EMP_IDENTI,
   USU_CODCOB,
   USU_PRGCRI,
   USU_USUCRI,
   USU_DATCRI,
   USU_USUBDD)

VALUES
  (SEQ_USUARI.NEXTVAL,
   'EDI',
   'EDI',
   'a0LaPh4VW5dFzJOdfy+SAw==',
   'ueverson.pedroso@rte.com.br',
   'N',
   '10',
   'N',
   'N',
   'S',
   'N',
   'N',
   'S',
   1,
   1,
   'UserDal',
   1,
   SYSDATE,
   'PLSQL');
   
   -- USUÁRIO: EDI
   -- SENHA: V9BQ2C3F