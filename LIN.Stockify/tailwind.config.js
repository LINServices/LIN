/** @type {import('tailwindcss').Config} */
module.exports = {
    content: ["D:/LIN/LIN Services/Components/LIN.Emma.UI/**/*{html,razor,js,cs}", "../**/*{html,razor,js,cs}", "D:/LIN/LIN Services/Components/LIN.Inventory.Shared/**/*{html,razor,js,cs}"],
    theme: {
        screens: {
            'sm': '640px',
            'md': '768px',
            'dl': '910px',
            'lg': '1024px',
            'xl': '1280px',
            '2xl': '1536px',
        },
        extend: {
            colors: {
                'money': "#52b34c",
                'current': {
                    '50': '#f3f7ee',
                    '100': '#e4edda',
                    '200': '#cbdeb8',
                    '300': '#aac88e',
                    '400': '#8bb16a',
                    '500': '#6e964c',
                    '600': '#54763a',
                    '700': '#40592e',
                    '800': '#374a2a',
                    '900': '#314027',
                    '950': '#172211'
                }
            },
            keyframes: {
                'fade-in': {
                    '0%': { opacity: '0' },
                    '100%': { opacity: '1' },
                },
            },
            animation: {
                'fade-in': 'fade-in 0.1s ease-in-out forwards',
            }
        }
    },
    plugins: []
}