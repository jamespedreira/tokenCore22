

--Cria a entidade relacional acao para objeto
create table edi_acaobj (
	aob_aca_identi integer not null,
	aob_obj_identi integer not null,
	foreign key (aob_aca_identi) references edi_acao(aca_identi),
	foreign key (aob_obj_identi) references tnu_objeto(obj_identi)
)
