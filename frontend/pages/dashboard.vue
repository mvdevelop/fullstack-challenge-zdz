<template>
  <div>
    <!-- Métricas -->
    <div class="pf-metrics">
      <div class="pf-metric">
        <div class="pf-metric-label">Saldo total</div>
        <div class="pf-metric-value" :class="saldoTotal >= 0 ? 'text-blue' : 'text-red'">R$ {{ formatarValor(saldoTotal) }}</div>
      </div>
      <div class="pf-metric">
        <div class="pf-metric-label">Receitas (mês)</div>
        <div class="pf-metric-value text-green">R$ {{ formatarValor(totalReceitas) }}</div>
      </div>
      <div class="pf-metric">
        <div class="pf-metric-label">Despesas (mês)</div>
        <div class="pf-metric-value text-red">R$ {{ formatarValor(totalDespesas) }}</div>
      </div>
    </div>

    <div class="pf-row-2col">
      <!-- Fluxo Mensal -->
      <div class="pf-card">
        <div class="pf-section-title">Fluxo mensal</div>
        <svg v-if="resumoMensal.length" class="pf-chart" viewBox="0 0 280 120">
          <defs>
            <linearGradient id="areaGrad" x1="0" y1="0" x2="0" y2="1">
              <stop offset="0%" stop-color="#1976D2" stop-opacity=".15"/>
              <stop offset="100%" stop-color="#1976D2" stop-opacity="0"/>
            </linearGradient>
          </defs>
          <polyline :points="chartLinePoints" fill="none" stroke="#1976D2" stroke-width="2" stroke-linejoin="round"/>
          <polygon :points="chartAreaPoints" fill="url(#areaGrad)"/>
          <line x1="10" y1="110" x2="250" y2="110" stroke="rgba(0,0,0,.1)" stroke-width=".5"/>
          <text v-for="(label, i) in chartLabels" :key="i" :x="chartLabelX[i]" y="118" font-size="9" fill="rgba(0,0,0,.4)">{{ label }}</text>
        </svg>
        <div v-else class="pf-chart-placeholder">
          <v-icon icon="mdi-chart-line" size="32" color="medium-emphasis" />
          <p class="text-medium-emphasis text-caption mt-2">Dados insuficientes</p>
        </div>
      </div>

      <!-- Gastos por Categoria -->
      <div class="pf-card">
        <div class="pf-section-title">Gastos por categoria</div>
        <div v-if="gastosPorCategoria.length">
          <div v-for="item in gastosPorCategoria.slice(0, 5)" :key="item.categoriaId" class="pf-row">
            <div class="pf-dot" :style="{ background: hexRgba(item.categoriaCor, .12), color: item.categoriaCor }">
              <v-icon icon="mdi-tag" size="14" />
            </div>
            <div class="pf-row-info">
              <div class="pf-row-title">{{ item.categoriaNome }}</div>
              <div class="pf-progress" style="margin-top:4px">
                <div class="pf-progress-fill" :style="{ width: item.percentual + '%', background: item.categoriaCor }" />
              </div>
            </div>
            <div class="pf-row-val" :style="{ color: item.categoriaCor }">R$ {{ formatarValor(item.total) }}</div>
          </div>
        </div>
        <div v-else class="text-center text-medium-emphasis py-6 text-caption">Nenhuma despesa este mês</div>
      </div>
    </div>

    <!-- Últimas Transações -->
    <div class="pf-card">
      <div class="d-flex align-center justify-space-between mb-3">
        <div class="pf-section-title mb-0">Últimas transações</div>
        <NuxtLink to="/transacoes" class="text-primary text-caption font-weight-medium text-decoration-none">Ver todas</NuxtLink>
      </div>
      <div v-if="ultimasTransacoes.length">
        <div v-for="t in ultimasTransacoes" :key="t.id" class="pf-row">
          <div class="pf-dot" :class="t.tipo === 'Receita' ? 'green' : 'red'">
            <v-icon :icon="t.tipo === 'Receita' ? 'mdi-arrow-down' : 'mdi-arrow-up'" size="14" />
          </div>
          <div class="pf-row-info">
            <div class="pf-row-title">{{ t.descricao }}</div>
            <div class="pf-row-sub">{{ formatarData(t.data) }} · {{ t.conta?.nome ?? '—' }}</div>
          </div>
          <div class="pf-row-val" :class="t.tipo === 'Receita' ? 'text-green' : 'text-red'">
            {{ t.tipo === 'Receita' ? '+' : '-' }} R$ {{ formatarValor(t.valor) }}
          </div>
        </div>
      </div>
      <div v-else class="text-center text-medium-emphasis py-6 text-caption">Nenhuma transação encontrada</div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { Orcamento, ResumoMensal, GastoPorCategoria } from '~/types'

definePageMeta({ layout: 'default', middleware: 'auth' })

const { get } = useApi()
const { saldoTotal } = useContas()
const transacaoStore = useTransacaoStore()
const { transacoes, totalReceitas, totalDespesas } = storeToRefs(transacaoStore)

const orcamentos = ref<Orcamento[]>([])
const resumoMensal = ref<ResumoMensal[]>([])
const gastosPorCategoria = ref<GastoPorCategoria[]>([])
const mesAtual = new Date().toISOString().slice(0, 7)
const ultimasTransacoes = computed(() => transacoes.value.slice(0, 6))

function formatarValor(v: number) { return v.toLocaleString('pt-BR', { minimumFractionDigits: 2 }) }
function formatarData(d: string) { return new Date(d).toLocaleDateString('pt-BR') }
function hexRgba(hex: string, alpha: number) {
  const r = parseInt(hex.slice(1, 3), 16), g = parseInt(hex.slice(3, 5), 16), b = parseInt(hex.slice(5, 7), 16)
  return `rgba(${r},${g},${b},${alpha})`
}

const chartLinePoints = computed(() => {
  if (resumoMensal.value.length < 2) return ''
  const data = resumoMensal.value.map(r => r.saldo)
  const max = Math.max(...data, 1), min = Math.min(...data, 0), range = max - min || 1, step = 240 / (data.length - 1)
  return data.map((v, i) => `${10 + i * step},${110 - ((v - min) / range) * 100}`).join(' ')
})
const chartAreaPoints = computed(() => {
  if (!chartLinePoints.value) return ''
  const pts = chartLinePoints.value.split(' ')
  return `${pts[0]} ${pts.slice(1).join(' ')} ${pts[pts.length - 1].split(',')[0]},110 10,110`
})
const chartLabels = computed(() => resumoMensal.value.map(r => ['Jan','Fev','Mar','Abr','Mai','Jun','Jul','Ago','Set','Out','Nov','Dez'][parseInt(r.mes.split('-')[1]) - 1]))
const chartLabelX = computed(() => { const n = resumoMensal.value.length; return n < 2 ? [] : Array.from({ length: n }, (_, i) => 10 + i * (240 / (n - 1))) })

onMounted(async () => {
  await transacaoStore.buscar({ dataInicio: `${mesAtual}-01`, dataFim: `${mesAtual}-31` })
  try {
    ;[orcamentos.value, resumoMensal.value, gastosPorCategoria.value] = await Promise.all([
      get<Orcamento[]>('/orcamentos', { mes: mesAtual }),
      get<ResumoMensal[]>('/relatorios/mensal', { meses: '6' }),
      get<GastoPorCategoria[]>('/relatorios/por-categoria', { mes: mesAtual }),
    ])
  } catch { /* ignore */ }
})
</script>

<style scoped>
.pf-metrics { display: grid; grid-template-columns: repeat(3, 1fr); gap: 12px; margin-bottom: 14px; }
.pf-metric { background: rgba(var(--v-theme-surface-variant), 0.3); border-radius: 10px; padding: 16px 18px; }
.pf-metric-label { font-size: .75rem; font-weight: 500; color: rgba(var(--v-theme-on-surface), .55); margin-bottom: 4px; }
.pf-metric-value { font-size: 1.4rem; font-weight: 700; letter-spacing: -.02em; }
.text-green { color: var(--pf-green) !important; } .text-red { color: var(--pf-red) !important; } .text-blue { color: var(--pf-accent) !important; }
.pf-row-2col { display: grid; grid-template-columns: 1fr 1fr; gap: 12px; margin-bottom: 14px; }
@media (max-width: 768px) { .pf-row-2col, .pf-metrics { grid-template-columns: 1fr; } }
.pf-chart { width: 100%; } .pf-chart-placeholder { display: flex; flex-direction: column; align-items: center; justify-content: center; min-height: 100px; }
.pf-progress { background: rgba(var(--v-theme-on-surface), .06); border-radius: 99px; height: 8px; overflow: hidden; }
.pf-progress-fill { height: 100%; border-radius: 99px; transition: width .4s ease; }
</style>