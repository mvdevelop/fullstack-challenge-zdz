export interface Transacao {
  id: string
  contaId: string
  conta?: Conta
  categoriaId: string
  categoria?: Categoria
  valor: number
  tipo: 'Receita' | 'Despesa' | 'Transferencia'
  descricao: string
  data: string
  status: 'Confirmada' | 'Pendente' | 'Cancelada'
  criadoEm: string
}

export interface TransacaoForm {
  contaId: string
  categoriaId: string
  valor: number
  tipo: 'Receita' | 'Despesa' | 'Transferencia'
  descricao: string
  data: string
  status?: 'Confirmada' | 'Pendente'
}

export interface FiltroTransacao {
  tipo?: string
  categoriaId?: string
  contaId?: string
  dataInicio?: string
  dataFim?: string
  busca?: string
}

export interface Conta {
  id: string
  usuarioId: string
  nome: string
  tipo: 'Corrente' | 'Poupanca' | 'Cartao' | 'Investimento' | 'Outro'
  saldoInicial: number
  saldoAtual: number
  moeda: string
  ativa: boolean
}

export interface ContaForm {
  nome: string
  tipo: 'Corrente' | 'Poupanca' | 'Cartao' | 'Investimento' | 'Outro'
  saldoInicial: number
  moeda: string
}

export interface Categoria {
  id: string
  usuarioId: string
  nome: string
  tipo: 'Receita' | 'Despesa' | 'Ambos'
  cor: string
  icone: string
}

export interface CategoriaForm {
  nome: string
  tipo: 'Receita' | 'Despesa' | 'Ambos'
  cor: string
  icone: string
}

export interface Orcamento {
  id: string
  contaId: string
  categoriaId: string
  categoria?: Categoria
  valorLimite: number
  mesReferencia: string
  valorGasto: number
  percentualUsado: number
}

export interface OrcamentoForm {
  contaId: string
  categoriaId: string
  valorLimite: number
  mesReferencia: string
}

export interface ResumoMensal {
  mes: string
  totalReceitas: number
  totalDespesas: number
  saldo: number
}

export interface GastoPorCategoria {
  categoriaId: string
  categoriaNome: string
  categoriaCor: string
  total: number
  percentual: number
}
