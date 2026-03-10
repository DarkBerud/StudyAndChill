import { Box } from '@mui/material';

const BorderImg = '/assets/LogoBorder.png';
const CoreImg = '/assets/LogoCore.png';

const AnimatedLogo = ({size = 250}: {size?: number}) => {
    return (
        <Box
            sx={{
                position : 'relative',
                width: size,
                height: size,
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                animation: 'float 6s ease-in-out infinite',
                '@keyframes float' : {
                    '0%': { transform: 'translateY(0px)' },
                    '50%': { transform: 'translateY(-15px)' },
                    '100%': { transform: 'translateY(0px)' },
                }
            }}
        >
        <Box
            component="img"
            src={BorderImg}
            alt="Study and Chill Border"
            sx={{
                position: 'absolute',
                width: '100%',
                height: '100%',
                animation: 'spin 20s linear infinite',
                '@keyframes spin': {
                    '0%': { transform: 'rotate(0deg)' },
                    '100%': { transform: 'rotate(360deg)' },
                },
            }}
            />
            <Box
                component="img"
                src={CoreImg}
                alt="Study and Chill Core"
                sx={{
                    position: 'absolute',
                    width: '100%',
                    height: '100%',
                    objectFit: 'contain',
                    zIndex: 2,
                }}
            />
            </Box>
    );
};

export default AnimatedLogo;