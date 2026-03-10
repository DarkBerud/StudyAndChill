import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { ThemeProvider, CssBaseline } from '@mui/material';
import theme from './theme';

import Home from './pages/Home';
import Login from './pages/Login';

const DashboardPlaceholder = () => (
  <div style={{ padding: 50, textAlign: 'center'}}>
    <p>Login Realizado com sucesso</p>
    <h1>Bem-vindo</h1>
  </div>
)

function App() {
  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <BrowserRouter>
        <Routes>
          <Route path='/' element={<Home />} />

          <Route path='/login' element={<Login />} />

          <Route path='/dashboard' element={<DashboardPlaceholder />} />
        </Routes>
      </BrowserRouter>
    </ThemeProvider>
  );
}

export default App;