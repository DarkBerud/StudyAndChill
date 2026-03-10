import { createTheme } from '@mui/material/styles';

const paletteColors = {
    orange: '#E65C00',
    darkBlue: '#16215B',
    salmonPink: '#EE6C85',
    magenta: '#E94560',
    lightBlueGrey: '#E4ECF7',
    backgroundCream: '#F6F5F0',
};

const theme = createTheme({
    palette: {
        primary: {
            main: paletteColors.orange,
        contrastText: '#fff',
        },
        secondary: {
            main: paletteColors.darkBlue
        },
        background: {
            default: paletteColors.backgroundCream,
            paper: '#ffffff',
        },
        custom: {
            pink: paletteColors.magenta,
            salmon: paletteColors.salmonPink,
            lightBlue:paletteColors.lightBlueGrey,
            purple: '#D4B2D8',
        },
        text: {
            primary: paletteColors.darkBlue,
            secondary: paletteColors.orange,
        },
    },
    typography: {
        fontFamily: '"Poppins", "Roboto", "Helvetica", "Arial", sans-serif',
        h1: {
            fontFamily: '"Bodoni Moda", serif',
            fontWeight: 700,
        },
        h2: {
            fontFamily: '"Bodoni Moda", serif',
            fontWeight: 700,
        },
        h3: {
            fontFamily: '"Great Vibes", cursive',
            color: paletteColors.orange,
        },
        button: {
            fontFamily: '"Poppons", sans-serif',
            fontWeight: 600,
            textTransform: 'none',
        },
    },
    shape: {
        borderRadius: 5,
    },
    components: {
        MuiGrid: {
            defaultProps: {
                
            }
        }
    }
});

declare module '@mui/material/styles' {
    interface Palette {
        custom: {
            pink: string;
            salmon: string;
            lightBlue: string;
            purple: string;
        };
    }
    interface PaletteOptions {
        custom?: {
            pink: string;
            salmon: string;
            lightBlue: string;
            purple: string;
        };
    }
}

export default theme;