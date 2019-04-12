-- Create table

create table public.EDI_CLPCCO
(
  clc_identi     INTEGER not null,
  clc_clp_identi INTEGER not null,
  clc_nome       VARCHAR(65),
  clc_e_mail     VARCHAR(255),
  clc_telcom     VARCHAR(35),
  clc_telcel     VARCHAR(35),
  clc_modenv     VARCHAR(1) not null,
  clc_ftpend     VARCHAR(255),
  clc_ftpenv     VARCHAR(255),
  clc_ftpret     VARCHAR(255),
  clc_ftpusu     VARCHAR(35),
  clc_ftpsen     VARCHAR(35),
  clc_ssl        VARCHAR(1) default 'N',
  clc_ematit     VARCHAR(65),
  clc_modati     VARCHAR(1) default 'N' not null,
  constraint CLC_PK primary key (CLC_IDENTI),
  constraint CLC_CLP_FK foreign key (CLC_CLP_IDENTI) references EDI_CCLPRC (CLP_IDENTI)
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
);

-- Add comments to the columns 
comment on column EDI_CLPCCO.clc_identi
  is 'Identificador do destinatário';
comment on column EDI_CLPCCO.clc_clp_identi
  is 'Identificador único do convênio-processo';
comment on column EDI_CLPCCO.clc_nome
  is 'Nome do destinatário';
comment on column EDI_CLPCCO.clc_e_mail
  is 'E-mail do destinatário';
comment on column EDI_CLPCCO.clc_telcom
  is 'Telefone comercial';
comment on column EDI_CLPCCO.clc_telcel
  is 'Telefone celular';
comment on column EDI_CLPCCO.clc_modenv
  is 'Modo de envio <E>mail/ <F>TP /  <I>nterno';
comment on column EDI_CLPCCO.clc_ssl
  is 'Servidor usa protocolo SSL <S> Sim / <N> Não';
comment on column EDI_CLPCCO.clc_ematit
  is 'Titulo do email';
comment on column EDI_CLPCCO.clc_modati
  is 'Ftp no modo ativo (N-NAO - S-SIM)';