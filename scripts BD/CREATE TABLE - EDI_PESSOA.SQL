create table public.edi_pessoa
(
  pes_identi   integer not null,
  pes_descri   character varying(65) not null,
  pes_cpfcnp   character varying(15) not null,
  CONSTRAINT edi_pessoa_pkey PRIMARY KEY (pes_identi)
 );
 
 -- Add comments to the columns 
comment on column EDI_CLPCCO.pes_identi
  is 'Identificador da Pessoa';
comment on column EDI_CLPCCO.pes_descri
  is 'Nome da pessoa';
comment on column EDI_CLPCCO.pes_cpfcnp
  is 'CPF/CNPJ da pessoa';
