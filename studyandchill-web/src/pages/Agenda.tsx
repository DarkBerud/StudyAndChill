import { useState, useEffect } from 'react';
import { Box, Typography, Paper, useTheme, Chip, Button, CircularProgress } from '@mui/material';
import Grid from '@mui/material/Grid';
import CalendarTodayIcon from '@mui/icons-material/CalendarToday';
import AccessTimeIcon from '@mui/icons-material/AccessTime';
import VideocamIcon from '@mui/icons-material/Videocam';
import PersonIcon from '@mui/icons-material/Person';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';

interface StudentSummary {
  id: number;
  name: string;
}

interface Classes {
  id: number;
  start: string;
  end: string;
  status: number;
  teacherId: number;
  teacherName: string;
  students: StudentSummary[];
  meetingLink?: string;
}

export default function Agenda() {
  const theme = useTheme();
  const navigate = useNavigate();

  const [classes, setClasses] = useState<Classes[]>([]);
  const [loading, setLoading] = useState(true);

  const getStatusConfig = (status: number) => {
    switch (status) {
      case 0: return { label: 'Agendada', color: 'primary' as const };
      case 1: return { label: 'Concluída', color: 'success' as const }; // Corrigido 'sucess'
      case 2: return { label: 'Falta (C/ Reposição)', color: 'warning' as const };
      case 3: return { label: 'Falta (S/ Reposição)', color: 'error' as const };
      case 4: return { label: 'Reposta', color: 'info' as const };
      default: return { label: 'Desconhecido', color: 'default' as const };
    }
  };

  useEffect(() => {
    const fetchClasses = async () => {
      try {
        const today = new Date();
        const thirtyDayLater = new Date();
        thirtyDayLater.setDate(today.getDate() + 30);

        const response = await api.get('/classsession', {
          params: {
            start: today.toISOString(),
            end: thirtyDayLater.toISOString()
          }
        });

        setClasses(response.data);

      } catch (error) {
        console.error('Erro ao buscar aulas:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchClasses();
  }, []);

  return (
    <Box>
      <Box sx={{ display: 'flex', gap: 3, alignItems: 'center', mb: 4 }}>
        <Typography variant="h4" sx={{ fontFamily: 'Bodoni Moda', color: theme.palette.primary.main, fontWeight: 'bold' }}>
          Minha Agenda
        </Typography>
        <Button variant="outlined" onClick={() => navigate('/dashboard')} sx={{ borderRadius: 3 }}>
          Voltar
        </Button>
      </Box>

      {loading ? (
        <Box sx={{ display: 'flex', justifyContent: 'center', mt: 10 }}>
          <CircularProgress />
        </Box>
      ) : classes.length === 0 ? (
        <Paper elevation={0} sx={{ p: 4, textAlign: 'center', bgcolor: 'transparent', border: `1px dashed ${theme.palette.divider}` }}>
          <CalendarTodayIcon sx={{ fontSize: 48, color: theme.palette.text.disabled, mb: 2 }} />
          <Typography variant='h6' color='textSecondary'>Nenhuma aula agendada para os próximos 30 dias</Typography>
        </Paper>
      ) : (
        <>
          <Typography variant="h6" sx={{ mb: 3, color: theme.palette.text.secondary }}>
            Próximas Aulas
          </Typography>

          <Grid container spacing={3}>
            {classes.map((classItem) => {
              const statusConfig = getStatusConfig(classItem.status);
              const formatedDate = new Date(classItem.start).toLocaleDateString('pt-BR', { day: '2-digit', month: 'short', year: 'numeric' });
              const start = new Date(classItem.start).toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' }); // Alterado para toLocaleTimeString

              return (
                <Grid size={{ xs: 12, md: 6, lg: 4 }} key={classItem.id}>
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
                    <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
                      <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                        <PersonIcon sx={{ color: theme.palette.primary.main }} />
                        <Typography variant="h6" sx={{ fontWeight: 'bold', color: theme.palette.text.primary, lineHeight: 1.2 }}>
                          {classItem.teacherName}
                        </Typography>
                      </Box>
                      <Chip
                        label={statusConfig.label}
                        size="small"
                        color={statusConfig.color}
                        sx={{ fontWeight: 'bold' }}
                      />
                    </Box>

                    <Box sx={{ display: 'flex', gap: 3, color: theme.palette.text.secondary, mt: 1 }}>
                      <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                        <CalendarTodayIcon fontSize="small" />
                        <Typography variant="body2">{formatedDate}</Typography>
                      </Box>
                      <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                        <AccessTimeIcon fontSize="small" />
                        <Typography variant="body2">{start}</Typography>
                      </Box>
                    </Box>

                    <Button
                      variant={classItem.meetingLink ? "contained" : "outlined"}
                      color="primary"
                      fullWidth
                      startIcon={<VideocamIcon />}
                      disabled={!classItem.meetingLink}
                      sx={{ mt: 1, borderRadius: 2 }}
                      onClick={() => classItem.meetingLink && window.open(classItem.meetingLink, '_blank')}
                    >
                      {classItem.meetingLink ? 'Acessar Aula' : 'Link Indisponível'}
                    </Button>
                  </Paper>
                </Grid>
              );
            })}
          </Grid>
        </>
      )}
    </Box>
  );
}