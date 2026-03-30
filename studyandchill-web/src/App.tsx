import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { ThemeProvider, CssBaseline } from '@mui/material';
import theme from './theme';

import ProtectedRoute from './components/ProtectedRoute';
import DashboardLayout from './components/DashboardLayout';
import Home from './pages/Home';
import Login from './pages/Login';
import Dashboard from './pages/Dashboard';
import Agenda from './pages/Agenda';
import Invoices from './pages/Invoices'
import Contracts from './pages/Contracts';


function App() {
  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <BrowserRouter>
        <Routes>
          <Route path='/' element={<Home />} />

          <Route path='/login' element={<Login />} />

          <Route path='/dashboard' element={<ProtectedRoute><DashboardLayout><Dashboard /></DashboardLayout></ProtectedRoute>} />

          <Route path='/agenda' element={<ProtectedRoute><DashboardLayout><Agenda /></DashboardLayout></ProtectedRoute>} />

          <Route path='/faturas' element={<ProtectedRoute><DashboardLayout><Invoices /></DashboardLayout></ProtectedRoute>} />

          <Route path='/contratos' element={<ProtectedRoute><DashboardLayout><Contracts /></DashboardLayout></ProtectedRoute>} />
        </Routes>
      </BrowserRouter>
    </ThemeProvider>
  );
}

export default App;