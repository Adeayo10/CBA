import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import "react-toastify/dist/ReactToastify.css";
import { ToastContainer } from "react-toastify";

import Login from "./Components/Login";


function App() {

  return (
    <>
      <h1>App</h1>
      <ToastContainer autoClose={3000} hideProgressBar />
      <Router>
        <Routes>
          <Route path="/Login" element={<Login />} />
        </Routes>
      </Router>
    </>
  );
}

export default App;