import Typography from '@mui/material/Typography';

const COMPANY_NAME = "Adeayo CBA"

export default function Copyright(props) {
  return (
    <Typography variant="body2" color="text.secondary" align="center" {...props}>
      {`Copyright © ${COMPANY_NAME} `}
      {new Date().getFullYear()}
      {'.'}
    </Typography>
  )
}