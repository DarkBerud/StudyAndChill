import { useState, useEffect } from 'react';
import { Box, Typography, Paper, useTheme, Button, CircularProgress, Avatar, Divider } from '@mui/material';
import Grid from '@mui/material/Grid';
import EmailIcon from '@mui/icons-material/Email';
import PhoneIcon from '@mui/icons-material/Phone';
import WhatsAppIcon from '@mui/icons-material/WhatsApp';
import PersonSearchIcon from '@mui/icons-material/PersonSearch';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';

interface StudentInfo {
    id: number;
    name: string;
    email: string;
    phone: string;
    activeContracts: number;
}

export default function Students() {
    const theme = useTheme();
    const navigate = useNavigate();

    const [students, setStudents] = useState<StudentInfo[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchStudents = async () => {
            try {
                const response = await api.get('/users/my-students');

                setStudents(response.data);

            } catch (error) {
                console.error('Erro ao buscar alunos:', error);
            } finally{
                setLoading(false);
            }
        };
        fetchStudents();
    }, []);

    const handleWhatsAppClick = (phone: string) => {
        const cleanPhone = phone.replace(/\D/g, '');
        window.open(`https://wa.me/${cleanPhone}`, '_blank');
    };

    return (
        <Box>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 3, mb: 4 }}>
                <Typography variant='h4' sx={{ fontFamily: 'Bodoni Moda', color: theme.palette.primary.main, fontWeight: 'bold' }}>
                    Meus Alunos
                </Typography>
                <Button variant="outlined" onClick={() => navigate('/dashboard')} sx={{ borderRadius: 3, size: 'small' }}>
                    Voltar
                </Button>
            </Box>

            {loading ? (
                <Box sx={{ display: 'flex', justifyContent: 'center', mt: 10 }}>
                    <CircularProgress />
                </Box>
            ) : students.length === 0 ? (
                <Paper elevation={0} sx={{ p: 4, textAlign: 'center', bgcolor: 'transparent', border: `1px dashed ${theme.palette.divider}` }}>
                    <PersonSearchIcon sx={{ fontSize: 48, color: theme.palette.text.disabled, mb: 2 }} />
                    <Typography variant='h6' color='textSecondary'>Nenhum aluno matriculado no momento.</Typography>
                </Paper>
            ) : (
                <Grid container spacing={3}>
                    {students.map((student) => (
                        <Grid size={{ xs: 12, md: 6, lg: 4 }} key={student.id}>
                            <Paper
                                elevation={0}
                                sx={{
                                    p: 3,
                                    borderRadius: theme.shape.borderRadius,
                                    border: `1px solid ${theme.palette.divider}`,
                                    display: 'flex',
                                    flexDirection: 'column',
                                    gap: 2,
                                    transition: 'box-shadow 0.2s',
                                    '&:hover': { boxShadow: 2 }
                                }}
                            >
                                <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
                                    <Avatar sx={{ bgcolor: theme.palette.primary.main, width: 56, height: 56 }}>
                                        {student.name.charAt(0).toUpperCase()}
                                    </Avatar>
                                    <Box>
                                        <Typography variant='h6' sx={{ fontWeight: 'bold', lineHeight: 1.2 }}>
                                            {student.name}
                                        </Typography>
                                        <Typography variant='body2' color='text.secondary'>
                                            {student.activeContracts} Contrato(s) Ativo(s)
                                        </Typography>
                                    </Box>
                                </Box>

                                <Divider sx={{ my: 1 }} />

                                <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1.5 }}>
                                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, color: theme.palette.text.secondary }}>
                                        <EmailIcon fontSize='small' />
                                        <Typography variant='body2' sx={{ wordBreak: 'break-all' }}>{student.email}</Typography>
                                    </Box>
                                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, color: theme.palette.text.secondary }}>
                                        <PhoneIcon fontSize='small' />
                                        <Typography variant='body2'>{student.phone || 'Não informado'}</Typography>
                                    </Box>
                                </Box>

                                <Button
                                    variant='contained'
                                    color='success'
                                    fullWidth
                                    startIcon={<WhatsAppIcon />}
                                    sx={{ mt: 1, borderRadius: 2 }}
                                    disabled={!student.phone}
                                    onClick={() => handleWhatsAppClick(student.phone)}
                                >
                                    Chamar no WhatsApp
                                </Button>
                            </Paper>
                        </Grid>
                    ))}
                </Grid>
            )}
        </Box>
    );
}