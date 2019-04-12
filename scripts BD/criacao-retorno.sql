
create table EDI_GRARTN(
	GTN_IDENTI INTEGER NOT NULL,
	GTN_CBL_IDENTI INTEGER NOT NULL,
	GTN_NAR_IDENTI INTEGER NOT NULL,
	GTN_CLI_IDENTI INTEGER NOT NULL, 
	GTN_CLP_IDENTI INTEGER NULL,
	GTN_DATAINI DATE NOT NULL,
	GTN_DATAFIN DATE NOT NULL,
	GTN_EDIPRC INTEGER NOT NULL,
	GTN_FLTCTR INTEGER NULL,
	GTN_TIPEXE INTEGER NOT NULL,
	GTN_PRGSTU INTEGER NOT NULL DEFAULT 0,
	constraint GTN_pk primary key (GTN_IDENTI),
	constraint GTN_CLI_fk foreign key (GTN_CLI_IDENTI) references EDI_CLIENT(CLI_IDENTI) 
);

COMMENT ON COLUMN EDI_GRARTN.GTN_TIPEXE IS '0 - Sistema, 1 - Manual';

comment on column EDI_GRARTN.GTN_FLTCTR
  is 'Filtro que representa o identificador único do conhecimento de transporte CT-e';

comment on column EDI_GRARTN.GTN_PRGSTU IS '0 - Não processado, 1 - Iniciado, 2 - Finalizado, 3 - Erro';
  
CREATE TABLE EDI_GTNCCO
(
  GTC_IDENTI     INTEGER NOT NULL,
  GTC_GTN_IDENTI INTEGER NOT NULL,
  GTC_NOME       VARCHAR(65) NULL,
  GTC_E_MAIL     VARCHAR(255),
  GTC_MODENV     VARCHAR(1) NOT NULL,
  GTC_FTPEND     VARCHAR(255),
  GTC_FTPENV     VARCHAR(255),
  GTC_FTPRET     VARCHAR(255),
  GTC_FTPUSU     VARCHAR(35),
  GTC_FTPSEN     VARCHAR(35),
  GTC_SSL        VARCHAR(1) DEFAULT 'N',
  GTC_EMATIT     VARCHAR(65),
  GTC_MODATI     VARCHAR(1) DEFAULT 'N' NOT NULL,
  CONSTRAINT GTC_PK PRIMARY KEY (GTC_IDENTI),
  CONSTRAINT GTC_GTN_FK FOREIGN KEY (GTC_GTN_IDENTI) REFERENCES EDI_GRARTN (GTN_IDENTI) ON DELETE CASCADE
);

-- Add comments to the columns 
comment on column EDI_GTNCCO.gtc_identi
  is 'Identificador do destinatário';
comment on column EDI_GTNCCO.gtc_gtn_identi
  is 'Identificador único do geração de retorno de arquivo';
comment on column EDI_GTNCCO.gtc_nome
  is 'Nome do destinatário';
comment on column EDI_GTNCCO.gtc_e_mail
  is 'E-mail do destinatário';
comment on column EDI_GTNCCO.gtc_modenv
  is 'Modo de envio <E>mail/ <F>TP /  <I>nterno';
comment on column EDI_GTNCCO.gtc_ssl
  is 'Servidor usa protocolo SSL <S> Sim / <N> Não';
comment on column EDI_GTNCCO.gtc_ematit
  is 'Titulo do email';
comment on column EDI_GTNCCO.gtc_modati
  is 'Ftp no modo ativo (N-NAO - S-SIM)';
  
  
CREATE TABLE EDI_GTNVLR(
	GTV_IDENTI 	 NOT NULL,
	GTV_GTN_IDENTI INTEGER NOT NULL,
	GTV_VALOR VARCHAR(255) NOT NULL,
	CONSTRAINT GTV_PK PRIMARY KEY (GTV_IDENTI), 
	CONSTRAINT GTV_GTN_FK FOREIGN KEY (GTV_GTN_IDENTI) REFERENCES EDI_GRARTN (GTN_IDENTI) ON DELETE CASCADE
);

CREATE SEQUENCE SEQ_EDI_GRARTN;
CREATE SEQUENCE SEQ_EDI_GTNCCO;
CREATE SEQUENCE SEQ_EDI_GTNVLR;
