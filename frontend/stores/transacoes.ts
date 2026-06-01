import { defineStore } from 'pinia'
import type { Transacao, TransacaoForm, FiltroTransacao } from '~/types'

export const useTransacaoStore = defineStore('transacoes', () => {
  const { get, post, put, del } = useApi()

  const transacoes = ref<Transacao[]>([])
  const loading = ref(false)
  const erro = ref<string | null>(null)

  const totalReceitas = computed(() =>
    transacoes.value.filter(t => t.tipo === 'Receita').reduce((acc, t) => acc + t.valor, 0)
  )
  const totalDespesas = computed(() =>
    transacoes.value.filter(t => t.tipo === 'Despesa').reduce((acc, t) => acc + t.valor, 0)
  )
  const saldo = computed(() => totalReceitas.value - totalDespesas.value)

  async function buscar(filtro?: FiltroTransacao) {
    loading.value = true
    erro.value = null
    try {
      const params: Record<string, string> = {}
      if (filtro?.tipo) params.tipo = filtro.tipo
      if (filtro?.categoriaId) params.categoriaId = filtro.categoriaId
      if (filtro?.contaId) params.contaId = filtro.contaId
      if (filtro?.dataInicio) params.dataInicio = filtro.dataInicio
      if (filtro?.dataFim) params.dataFim = filtro.dataFim
      if (filtro?.busca) params.busca = filtro.busca
      transacoes.value = await get<Transacao[]>('/transacoes', params)
    } catch (e: any) { erro.value = e.message }
    finally { loading.value = false }
  }

  async function criar(form: TransacaoForm) {
    loading.value = true
    erro.value = null
    try {
      const nova = await post<Transacao>('/transacoes', form)
      transacoes.value.unshift(nova)
      return nova
    } catch (e: any) { erro.value = e.message; throw e }
    finally { loading.value = false }
  }

  async function atualizar(id: string, form: TransacaoForm) {
    loading.value = true
    erro.value = null
    try {
      const atualizada = await put<Transacao>(`/transacoes/${id}`, form)
      const idx = transacoes.value.findIndex(t => t.id === id)
      if (idx !== -1) transacoes.value[idx] = atualizada
      return atualizada
    } catch (e: any) { erro.value = e.message; throw e }
    finally { loading.value = false }
  }

  async function excluir(id: string) {
    loading.value = true
    erro.value = null
    try {
      await del(`/transacoes/${id}`)
      transacoes.value = transacoes.value.filter(t => t.id !== id)
    } catch (e: any) { erro.value = e.message; throw e }
    finally { loading.value = false }
  }

  return { transacoes, loading, erro, totalReceitas, totalDespesas, saldo, buscar, criar, atualizar, excluir }
})