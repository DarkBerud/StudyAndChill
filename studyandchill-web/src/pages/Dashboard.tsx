import { Typography, Paper, Box, useTheme} from '@mui/material';
import Grid from '@mui/material/Grid';
import PeopleAltIcon from '@mui/icons-material/PeopleAlt';
import CalendarMonthIcon from '@mui/icons-material/CalendarMonth';
import AttachMoneyIcon from '@mui/icons-material/AttachMoney';
import MenuBookIcon from '@mui/icons-material/MenuBook';
import SchoolIcon from '@mui/icons-material/School';
import DescriptionIcon from '@mui/icons-material/Description';
import { useNavigate } from 'react-router-dom';

interface DashboardCard {
    title: string;
    icon: React.ReactNode;
    color: string;
    textColor: string;
}

const Dashboard = () => {
    const theme = useTheme();
    const navigate = useNavigate();

    const userStr = localStorage.getItem('user');
    const user = userStr ? JSON.parse(userStr) : null;
    const role = user?.role || 'Student';

    let cards: DashboardCard[] = [];



    if (role === 'Admin') {
    cards = [
      { title: 'Gestão\nde Alunos', icon: <PeopleAltIcon sx={{ fontSize: 48 }}/>, color: theme.palette.custom.lightBlue, textColor: theme.palette.primary.main },
      { title: 'Professores', icon: <SchoolIcon sx={{ fontSize: 48 }}/>, color: theme.palette.custom.salmon, textColor: '#fff' },
      { title: 'Financeiro', icon: <AttachMoneyIcon sx={{ fontSize: 48 }}/>, color: theme.palette.custom.purple, textColor: '#fff' },
      { title: 'Contratos', icon: <DescriptionIcon sx={{ fontSize: 48 }}/>, color: theme.palette.custom.pink, textColor: '#fff' },
    ];
  } else if (role === 'Teacher') {
    cards = [
      { title: 'Minhas Aulas', icon: <CalendarMonthIcon sx={{ fontSize: 48 }}/>, color: theme.palette.custom.salmon, textColor: '#fff' },
      { title: 'Meus Alunos', icon: <PeopleAltIcon sx={{ fontSize: 48 }}/>, color: theme.palette.custom.lightBlue, textColor: theme.palette.primary.main },
      { title: 'Minhas Comissões', icon: <AttachMoneyIcon sx={{ fontSize: 48 }}/>, color: theme.palette.custom.purple, textColor: '#fff' },
    ];
  } else {
    cards = [
      { title: 'Agenda\nde Aulas', icon: <CalendarMonthIcon sx={{ fontSize: 48 }}/>, color: theme.palette.custom.salmon, textColor: '#fff' },
      { title: 'Contrato', icon: <MenuBookIcon sx={{ fontSize: 48 }}/>, color: theme.palette.custom.pink, textColor: '#fff' },
      { title: 'Minhas Faturas', icon: <DescriptionIcon sx={{ fontSize: 48 }}/>, color: theme.palette.custom.lightBlue, textColor: theme.palette.primary.main },
    ];
  }

    return (
        <Box>
            <Typography variant='h4' sx={{ mb: 4, fontFamily: 'Bodoni Moda', color: theme.palette.primary.main, fontWeight: 'bold'}}>
                Visão Geral
            </Typography>

            <Grid container spacing={4}>
                {cards.map((card, index) => (
                    <Grid size={{ xs: 12, sm: 6, md: 3}} key={index}>
                        <Paper
                            elevation={0}
                            sx={{
                                p: 3,
                                height: 220,
                                bgcolor: card.color,
                                borderRadius: theme.shape.borderRadius,
                                display: 'flex',
                                flexDirection: 'column',
                                justifyContent: 'space-between',
                                cursor: 'pointer',
                                transition: 'transform 0.2s',
                                '&:hover': { transform: 'scale(1.05)'},
                                position: 'relative',
                                overflow: 'hidden'
                            }}
                            onClick={() => {
                                 if (card.title === 'Agenda\nde Aulas' || card.title === 'Minhas Aulas') {
                                    navigate('/agenda');
                                 } else if (card.title === 'Financeiro' || card.title == 'Minhas Faturas'){
                                    navigate('/faturas');
                                } else if (card.title ==='Contrato') {
                                    navigate('/contratos');
                                } else {
                                    alert(`Navegar para ${card.title} (Em construção)`);
                                }
              }}
                        >
                            <Box sx={{ color: card.textColor, opacity: 0.8}}>
                                {card.icon}
                            </Box>

                            <Typography variant='h5' sx={{ color: card.textColor, fontFamily: 'Bodoni Moda', fontWeight: 'bold', lineHeight: 1.2, whiteSpace: 'pre-line'}}>
                                {card.title}
                            </Typography>
                        </Paper>
                    </Grid>
                ))}
            </Grid>
        </Box>
    );
};

export default Dashboard;