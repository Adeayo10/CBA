import {
  BrowserRouter as Router,
  Route,
  Routes,
  Navigate,
} from "react-router-dom";
import "react-toastify/dist/ReactToastify.css";
import CssBaseline from "@mui/material/CssBaseline";
import { ToastContainer } from "react-toastify";

import Login from "./Pages/Login";
import Dashboard from "./Pages/Dashboard/Dashboard";
import Copyright from "./Components/Copyright";
import Users from "./Pages/Dashboard/Users/Users";
import UserRoles from "./Pages/Dashboard/UserRoles";
import ForgotPassword from "./Pages/ForgotPassword";
import { Typography } from "@mui/material";
import ResetPassword from "./Pages/ResetPassword";
import Profile from "./Pages/Dashboard/Profile";
import VerifyToken from "./Pages/VerifyToken";
import AccountStatement from "./Pages/Dashboard/AccountStatement";
import CustomerAccount from "./Pages/Dashboard/CustomerAccount";
import CustomerInformation from "./Pages/Dashboard/CustomerInformation";
import GeneralLedger from "./Pages/Dashboard/GeneralLedger/GeneralLedger";
import { ROUTES, ACCOUNT_TYPES, POSTING_TYPES } from "./utils/constants";
import Accounts from "./Pages/Dashboard/Accounts/Accounts";
import { LocalizationProvider } from "@mui/x-date-pickers";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import CreatePosting from "./Pages/Dashboard/Postings/CreatePosting";
import NotFound from "./Pages/NotFound";
import Postings from "./Pages/Dashboard/Postings/Postings";
import DashboardHome from "./Pages/Dashboard/DashBoardHome";

function App() {
  return (
    <>
      <LocalizationProvider dateAdapter={AdapterDayjs}>
        <ToastContainer autoClose={3000} hideProgressBar />
        <CssBaseline />
        <Router>
          <Routes>
            <Route
              path={ROUTES.BASE_PATH}
              element={<Navigate to={ROUTES.LOGIN} replace />}
            />
            <Route path={ROUTES.LOGIN} element={<Login />} />
            <Route path={ROUTES.VERIFY_TOKEN} element={<VerifyToken />} />
            <Route path={ROUTES.FORGOT_PASSWORD} element={<ForgotPassword />} />
            <Route path={ROUTES.FORGOT_PASSWORD} element={<ResetPassword />} />

            <Route path={ROUTES.DASHBOARD} element={<Dashboard />}>
              <Route path="" element={<DashboardHome />} />
              <Route path={ROUTES.USERS} element={<Users />} />
              <Route path={ROUTES.USER_ROLES} element={<UserRoles />} />
              <Route path={ROUTES.PROFILE} element={<Profile />} />
              <Route
                path={ROUTES.ACCOUNT_STATEMENT}
                element={<AccountStatement />}
              />
              <Route
                path={ROUTES.CURRENT_ACCOUNTS}
                element={<Accounts accountType={ACCOUNT_TYPES.CURRENT} />}
              />
              <Route
                path={ROUTES.SAVINGS_ACCOUNTS}
                element={<Accounts accountType={ACCOUNT_TYPES.SAVINGS} />}
              />
              <Route
                path={ROUTES.CUSTOMER_ACCOUNTS}
                element={<CustomerAccount />}
              />
              <Route
                path={ROUTES.CUSTOMER_INFO}
                element={<CustomerInformation />}
              />
              <Route path={ROUTES.GENERAL_LEDGER} element={<GeneralLedger />} />

              <Route path={ROUTES.CREATE_POSTING} element={<CreatePosting />} />
              <Route
                path={ROUTES.TRANSFERS}
                element={<Postings postingType={POSTING_TYPES.TRANSFER} />}
              />
              <Route
                path={ROUTES.DEPOSITS}
                element={<Postings postingType={POSTING_TYPES.DEPOSIT} />}
              />
              <Route
                path={ROUTES.WITHDRAWALS}
                element={<Postings postingType={POSTING_TYPES.WITHDRAWAL} />}
              />
            </Route>
            <Route
              path={"*"}
              element={<Navigate to={ROUTES.NOT_FOUND} replace />}
            />
            <Route path={ROUTES.NOT_FOUND} element={<NotFound />} />
          </Routes>
        </Router>
      </LocalizationProvider>
    </>
  );
}

export default App;
