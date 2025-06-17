import React, { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";

export default function DeleteCycle() {
    const { id } = useParams();
    const navigate = useNavigate();
    const [cycle, setCycle] = useState(null);

    useEffect(() => {
        fetch(`/api/CyclesApi/${id}`)
            .then((res) => {
                if (!res.ok) throw new Error("Cycle not found");
                return res.json();
            })
            .then((data) => setCycle(data))
            .catch((error) => {
                console.error(error);
                alert("Unable to load cycle");
                navigate("/");
            });
    }, [id, navigate]);

    const handleDelete = () => {
        fetch(`/api/CyclesApi/${id}`, {
            method: "DELETE",
        })
            .then((res) => {
                if (!res.ok) throw new Error("Delete failed");
                navigate("/");
            })
            .catch((err) => {
                console.error(err);
                alert("Failed to delete cycle");
            });
    };

    if (!cycle) return <p>Loading...</p>;

    return (
        <div className="container mt-4">
            <h1>Delete</h1>
            <h3>Are you sure you want to delete this?</h3>
            <div className="card p-3 shadow-sm">
                <p><strong>Cycle Length:</strong> {cycle.cycleLength} days</p>
                <p><strong>Last Period Start:</strong> {new Date(cycle.lastPeriodStartDate).toLocaleDateString()}</p>
                <p><strong>Next Period:</strong> {new Date(cycle.nextPeriodStartDate).toLocaleDateString()}</p>
            </div>
            <div className="mt-3">
                <button onClick={handleDelete} className="btn btn-danger me-2">Delete</button>
                <button onClick={() => navigate("/")} className="btn btn-secondary">Back to List</button>
            </div>
        </div>
    );
}
