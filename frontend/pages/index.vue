<template>
  <div class="landing-page">
    <!-- Hero -->
    <section class="landing-hero">
      <div class="landing-container">
        <div class="landing-hero-content">
          <div class="landing-badge">
            <v-icon icon="mdi-wallet" size="16" />
            <span>Controle Financeiro</span>
          </div>
          <h1 class="landing-title">
            Suas finanças sob
            <span class="landing-title-accent">controle</span>
          </h1>
          <p class="landing-subtitle">
            Gerencie receitas, despesas, orçamentos e contas bancárias em um só lugar. Simples, rápido e visual.
          </p>
          <div class="landing-actions">
            <NuxtLink to="/login" class="landing-btn landing-btn-primary">
              Começar agora
              <v-icon icon="mdi-arrow-right" size="16" />
            </NuxtLink>
            <a href="#features" class="landing-btn landing-btn-secondary">
              Saiba mais
            </a>
          </div>
        </div>
        <div class="landing-hero-visual">
          <div class="landing-mockup">
            <div class="landing-mockup-card">
              <div class="landing-mockup-row">
                <div class="landing-mockup-dot green" />
                <div class="landing-mockup-line" />
              </div>
              <div class="landing-mockup-row">
                <div class="landing-mockup-dot red" />
                <div class="landing-mockup-line short" />
              </div>
              <div class="landing-mockup-row">
                <div class="landing-mockup-dot accent" />
                <div class="landing-mockup-line" />
              </div>
            </div>
            <div class="landing-mockup-stats">
              <div class="landing-mockup-metric">
                <span class="landing-mockup-label">Saldo</span>
                <span class="landing-mockup-value text-pf-accent">R$ 8.420</span>
              </div>
              <div class="landing-mockup-metric">
                <span class="landing-mockup-label">Receitas</span>
                <span class="landing-mockup-value text-pf-green">R$ 6.800</span>
              </div>
              <div class="landing-mockup-metric">
                <span class="landing-mockup-label">Despesas</span>
                <span class="landing-mockup-value text-pf-red">R$ 4.130</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Features -->
    <section id="features" class="landing-features">
      <div class="landing-container">
        <h2 class="landing-section-title">Funcionalidades</h2>
        <div class="landing-features-grid">
          <div class="landing-feature-card">
            <div class="landing-feature-icon">
              <v-icon icon="mdi-view-dashboard" size="24" />
            </div>
            <h3>Dashboard</h3>
            <p>Visualize seu saldo, receitas e despesas com gráficos e indicadores visuais em tempo real.</p>
          </div>
          <div class="landing-feature-card">
            <div class="landing-feature-icon">
              <v-icon icon="mdi-swap-horizontal" size="24" />
            </div>
            <h3>Transações</h3>
            <p>Registre receitas, despesas e transferências com categoria, conta e data. Filtre e busque facilmente.</p>
          </div>
          <div class="landing-feature-card">
            <div class="landing-feature-icon">
              <v-icon icon="mdi-target" size="24" />
            </div>
            <h3>Orçamentos</h3>
            <p>Defina limites mensais por categoria e acompanhe o progresso com barras visuais coloridas.</p>
          </div>
          <div class="landing-feature-card">
            <div class="landing-feature-icon">
              <v-icon icon="mdi-bank" size="24" />
            </div>
            <h3>Contas</h3>
            <p>Gerencie múltiplas contas bancárias, cartões e investimentos com saldo atualizado automaticamente.</p>
          </div>
          <div class="landing-feature-card">
            <div class="landing-feature-icon">
              <v-icon icon="mdi-tag-multiple" size="24" />
            </div>
            <h3>Categorias</h3>
            <p>Organize suas transações com categorias personalizáveis, ícones e cores para fácil identificação.</p>
          </div>
          <div class="landing-feature-card">
            <div class="landing-feature-icon">
              <v-icon icon="mdi-chart-bar" size="24" />
            </div>
            <h3>Relatórios</h3>
            <p>Acompanhe resumos mensais e gastos por categoria com períodos configuráveis.</p>
          </div>
        </div>
      </div>
    </section>

    <!-- CTA -->
    <section class="landing-cta">
      <div class="landing-container">
        <div class="landing-cta-content">
          <h2>Comece agora — é grátis</h2>
          <p>Crie sua conta e tenha controle total das suas finanças em poucos minutos.</p>
          <NuxtLink to="/login" class="landing-btn landing-btn-primary landing-btn-large">
            Criar minha conta
            <v-icon icon="mdi-arrow-right" size="18" />
          </NuxtLink>
        </div>
      </div>
    </section>

    <!-- Footer -->
    <footer class="landing-footer">
      <div class="landing-container">
        <div class="d-flex align-center ga-2">
          <v-icon icon="mdi-wallet" size="16" color="primary" />
          <span class="font-weight-medium">Controle Financeiro</span>
        </div>
        <span class="text-medium-emphasis">Projeto de estudo — 2025</span>
      </div>
    </footer>
  </div>
</template>

<script setup lang="ts">
definePageMeta({ layout: false })
</script>

<style scoped>
/* ===== Layout ===== */
.landing-page {
  min-height: 100vh;
  background: var(--v-theme-background);
  color: var(--v-theme-on-background);
  font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;
}

.landing-container {
  max-width: 960px;
  margin: 0 auto;
  padding: 0 20px;
}

/* ===== Hero ===== */
.landing-hero {
  padding: 80px 0 60px;
  background: linear-gradient(135deg, rgba(25,118,210,.06) 0%, rgba(25,118,210,.01) 100%);
}
.v-theme--dark .landing-hero {
  background: linear-gradient(135deg, rgba(100,181,246,.06) 0%, rgba(100,181,246,.01) 100%);
}

.landing-hero-content {
  max-width: 560px;
}

.landing-badge {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 6px 14px;
  border-radius: 99px;
  background: rgba(var(--pf-accent-rgb), .1);
  color: var(--pf-accent);
  font-size: .75rem;
  font-weight: 600;
  letter-spacing: .03em;
  text-transform: uppercase;
  margin-bottom: 20px;
}

.landing-title {
  font-size: 2.8rem;
  font-weight: 700;
  line-height: 1.15;
  letter-spacing: -.03em;
  margin: 0 0 16px;
}
.landing-title-accent {
  color: var(--pf-accent);
}

.landing-subtitle {
  font-size: 1.1rem;
  line-height: 1.6;
  color: rgba(0,0,0,.55);
  margin: 0 0 32px;
  max-width: 440px;
}
.v-theme--dark .landing-subtitle {
  color: rgba(255,255,255,.55);
}

.landing-actions {
  display: flex;
  gap: 12px;
  flex-wrap: wrap;
}

/* Buttons */
.landing-btn {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  padding: 12px 24px;
  border-radius: 10px;
  font-size: .95rem;
  font-weight: 600;
  text-decoration: none;
  transition: all .15s;
  cursor: pointer;
}

.landing-btn-primary {
  background: var(--pf-accent);
  color: #fff;
  border: none;
}
.landing-btn-primary:hover {
  opacity: .88;
  transform: translateY(-1px);
}

.landing-btn-secondary {
  background: transparent;
  color: var(--pf-accent);
  border: 1px solid var(--pf-accent);
}
.landing-btn-secondary:hover {
  background: rgba(var(--pf-accent-rgb), .06);
}

.landing-btn-large {
  padding: 14px 32px;
  font-size: 1rem;
}

/* ===== Hero Visual ===== */
.landing-hero {
  position: relative;
}

.landing-hero-visual {
  display: none;
}
@media (min-width: 768px) {
  .landing-hero .landing-container {
    display: flex;
    gap: 48px;
    align-items: center;
  }
  .landing-hero-visual {
    display: block;
    flex-shrink: 0;
  }
}

/* Mockup */
.landing-mockup {
  background: var(--v-theme-surface);
  border: 1px solid rgba(0,0,0,.08);
  border-radius: 16px;
  padding: 20px;
  box-shadow: 0 4px 20px rgba(0,0,0,.06);
  width: 320px;
}
.v-theme--dark .landing-mockup {
  border-color: rgba(255,255,255,.08);
}

.landing-mockup-card {
  background: rgba(0,0,0,.02);
  border-radius: 10px;
  padding: 14px;
  margin-bottom: 14px;
}
.v-theme--dark .landing-mockup-card {
  background: rgba(255,255,255,.03);
}

.landing-mockup-row {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 8px 0;
}
.landing-mockup-row:not(:last-child) {
  border-bottom: 1px solid rgba(0,0,0,.05);
}
.v-theme--dark .landing-mockup-row:not(:last-child) {
  border-bottom-color: rgba(255,255,255,.05);
}

.landing-mockup-dot {
  width: 28px;
  height: 28px;
  border-radius: 8px;
  flex-shrink: 0;
}
.landing-mockup-dot.green { background: rgba(var(--pf-green-rgb), .12); }
.landing-mockup-dot.red { background: rgba(var(--pf-red-rgb), .12); }
.landing-mockup-dot.accent { background: rgba(var(--pf-accent-rgb), .12); }

.landing-mockup-line {
  height: 8px;
  border-radius: 4px;
  background: rgba(0,0,0,.08);
  flex: 1;
}
.landing-mockup-line.short { flex: .6; }
.v-theme--dark .landing-mockup-line { background: rgba(255,255,255,.08); }

.landing-mockup-stats {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 8px;
}

.landing-mockup-metric {
  background: rgba(0,0,0,.03);
  border-radius: 8px;
  padding: 10px;
  text-align: center;
}
.v-theme--dark .landing-mockup-metric {
  background: rgba(255,255,255,.04);
}

.landing-mockup-label {
  display: block;
  font-size: .6rem;
  color: rgba(0,0,0,.45);
  margin-bottom: 4px;
  text-transform: uppercase;
  letter-spacing: .04em;
}
.v-theme--dark .landing-mockup-label {
  color: rgba(255,255,255,.45);
}

.landing-mockup-value {
  display: block;
  font-size: .8rem;
  font-weight: 700;
  letter-spacing: -.02em;
}

/* ===== Features ===== */
.landing-features {
  padding: 80px 0;
}

.landing-section-title {
  font-size: 1.8rem;
  font-weight: 700;
  text-align: center;
  margin: 0 0 48px;
  letter-spacing: -.02em;
}

.landing-features-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 16px;
}
@media (max-width: 768px) {
  .landing-features-grid { grid-template-columns: repeat(2, 1fr); }
}
@media (max-width: 480px) {
  .landing-features-grid { grid-template-columns: 1fr; }
}

.landing-feature-card {
  background: var(--v-theme-surface);
  border: 1px solid rgba(0,0,0,.06);
  border-radius: 14px;
  padding: 24px 20px;
  text-align: center;
  transition: box-shadow .15s, transform .15s;
}
.landing-feature-card:hover {
  box-shadow: 0 2px 8px rgba(0,0,0,.06), 0 8px 24px rgba(0,0,0,.04);
  transform: translateY(-2px);
}
.v-theme--dark .landing-feature-card {
  border-color: rgba(255,255,255,.06);
}

.landing-feature-icon {
  width: 48px;
  height: 48px;
  border-radius: 12px;
  background: rgba(var(--pf-accent-rgb), .1);
  color: var(--pf-accent);
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto 14px;
}

.landing-feature-card h3 {
  font-size: .95rem;
  font-weight: 600;
  margin: 0 0 8px;
}

.landing-feature-card p {
  font-size: .82rem;
  line-height: 1.5;
  color: rgba(0,0,0,.5);
  margin: 0;
}
.v-theme--dark .landing-feature-card p {
  color: rgba(255,255,255,.5);
}

/* ===== CTA ===== */
.landing-cta {
  padding: 60px 0 80px;
  text-align: center;
  background: linear-gradient(135deg, rgba(25,118,210,.04) 0%, rgba(25,118,210,.01) 100%);
}
.v-theme--dark .landing-cta {
  background: linear-gradient(135deg, rgba(100,181,246,.04) 0%, rgba(100,181,246,.01) 100%);
}

.landing-cta-content h2 {
  font-size: 1.6rem;
  font-weight: 700;
  margin: 0 0 12px;
  letter-spacing: -.02em;
}

.landing-cta-content p {
  font-size: 1rem;
  color: rgba(0,0,0,.5);
  margin: 0 0 24px;
}
.v-theme--dark .landing-cta-content p {
  color: rgba(255,255,255,.5);
}

/* ===== Footer ===== */
.landing-footer {
  padding: 20px 0;
  border-top: 1px solid rgba(0,0,0,.06);
}
.v-theme--dark .landing-footer {
  border-top-color: rgba(255,255,255,.06);
}

.landing-footer .landing-container {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

/* ===== Responsive ===== */
@media (max-width: 768px) {
  .landing-hero { padding: 48px 0 40px; }
  .landing-title { font-size: 2rem; }
  .landing-subtitle { font-size: 1rem; }
  .landing-section-title { font-size: 1.4rem; }
}
</style>
