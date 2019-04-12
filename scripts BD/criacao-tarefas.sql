select * from edi_proved;

--Criação da tabela de tarefas
create table edi_taref (
	edi_identi integer primary key,
	edi_nome character varying(50),
	edi_descri character varying(200)
);
select * from edi_taref;

--Criar sequencia da entidade tarefa
CREATE SEQUENCE public.seq_edi_gtnsts
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
	
	
	
create table edi_gtnsts (
	gts_identi integer not null,
	gts_gtn_identi integer not null,
	gts_prgstu integer not null,
	gts_dathor timestamp not null,
	
	PRIMARY KEY (gts_identi),
	FOREIGN KEY (gts_gtn_identi) REFERENCES edi_grartn (gtn_identi)
)


insert into edi_gtnsts values (1 , 39, 1, '05-02-2019', '12:00:00')
insert into edi_gtnsts values (2 , 39, 2, '05-02-2019', '12:04:00')