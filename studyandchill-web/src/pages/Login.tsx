import React, { useState } from 'react';
import { Container, Paper, TextField, Button, Typography, Box, Alert, CircularProgress, useTheme } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';
import axios from 'axios';
import AnimatedLogo from '../components/AnimatedLogo';

const Login = () => {
    const theme = useTheme();
    const navigate = useNavigate();

    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);

    const handleLogin = async (e: React.FormEvent) => {
        e.preventDefault();
        setError('');
        setLoading(true);

        try {
            const response = await api.post('/auth/login', { email, password});

            const token = response.data.token;
            localStorage.setItem('token', token);
            localStorage.setItem('user', JSON.stringify(response.data));

            navigate('/dashboard');
        } catch (err: unknown) {
            console.error(err);

            if (axios.isAxiosError(err) && err.response){
                const backendErrors= err.response.data.errors;
                if (backendErrors)
                {
                    if (backendErrors.Email) {
                        setError(backendErrors.Email[0]);
                    } else if (backendErrors.Password) {
                        setError(backendErrors.Password[0]);
                    } else {
                        setError('Erro de validação nos campos.');
                    }
                } else {
                    setError('E-mail ou senha incorretos.');
                }
            } else {
                setError('Ocorreu um erro de conexão.');
            }
            
        } finally {
            setLoading(false);
        }
    };

    return (
        <Container component='main' maxWidth='xs' sx={{height: '100vh', display: 'flex', alignItems: 'center', justifyContent: 'center'}}>
            <Paper
                elevation={0}
                sx={{
                    p: 4,
                    display: 'flex',
                    flexDirection: 'column',
                    alignItems: 'center',
                    bgcolor: '#fff',
                    borderRadius: theme.shape.borderRadius,
                    border: '2px solid ${theme.palette.custom.lightBlue}',
                    width: '100%'
                }}
            >
                <Box sx={{ mb: 2}}>
                    <AnimatedLogo size={100} />
                </Box>

                <Typography component='h1' variant='h4' sx={{mb: 3, fontFamily: 'Bodoni Moda', color: theme.palette.primary.main }}>
                    Welcome
                </Typography>

                {error && <Alert severity='error' sx={{ width: '100%', mb: 2}}>{error}</Alert>}

                <Box component='form' onSubmit={handleLogin} sx={{width: '100%'}}>
                    <TextField
                    margin='normal'
                    fullWidth
                    label='E-mail'
                    autoComplete='email'
                    autoFocus
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    sx={{
                        '&. MuiOutlinedInput-root': {borderRadius: 3}
                    }}
                    />
                    <TextField
                        margin='normal'
                        fullWidth
                        label='Senha'
                        type='password'
                        autoComplete='current-password'
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        sx={{
                            '&. MuiOutlinedInput-root': {borderRadius: 3}
                        }}
                    />

                    <Button
                        type='submit'
                        fullWidth
                        variant='contained'
                        disabled={loading}
                        sx={{
                            mt: 3,
                            mb: 2,
                            py: 1.5,
                            borderRadius: 3,
                            bgcolor: theme.palette.primary.main,
                            fontSize: '1.1rem',
                            '&:hover': { bgcolor: theme.palette.secondary.main}
                        }}
                    >
                        {loading ? <CircularProgress  size={24} color='inherit' /> : 'Entrar'}
                    </Button>

                    <Button
                        fullWidth
                        onClick={() => navigate("/")}
                        sx={{ color: theme.palette.text.secondary}}
                    >
                        Voltar
                    </Button>
                </Box>
            </Paper>
        </Container>
    );
};

export default Login;