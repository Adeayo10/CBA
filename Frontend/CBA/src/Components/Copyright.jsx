import Typography from "@mui/material/Typography";

const COMPANY_NAME = "Adeayo CBA";

export default function Copyright(props) {
  return (
    <Typography
      variant="body2"
      color="text.secondary"
      align="center"
      {...props}
      sx={{ mt: 3, mb: 2 }}
    >
      {`Copyright © ${COMPANY_NAME} `}
      {new Date().getFullYear()}
      {"."}
    </Typography>
  );
}
