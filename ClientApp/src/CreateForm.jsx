import React, { useState } from "react";

const CreateCycleForm = ({ onCreated }) => {
    const [form, setForm] = useState({
        cycleLength: 28,
        lastPeriodStartDate: "",
    });

    const [errors, setErrors] = useState({});

    const handleChange = (e) => {
        setForm({
            ...form,
            [e.target.name]: e.target.value,
        });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        setErrors({}); // Reset errors

        const payload = {
            cycleLength: parseInt(form.cycleLength),
            lastPeriodStartDate: form.lastPeriodStartDate,
        };

        try {
            const response = await fetch("/MyCycles/Create", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(payload),
            });

            if (response.ok) {
                const result = await response.json();
                onCreated(); // Reload or notify
            } else {
                const errorText = await response.text();
                console.error("Create failed:", errorText);
                setErrors({ general: "Failed to create cycle. Please check inputs." });
            }
        } catch (error) {
            console.error("Error submitting form", error);
            setErrors({ general: "Unexpected error occurred." });
        }
    };

    return (
        <div className="card p-4 mb-4 shadow-sm">
            <h4>Create New Cycle</h4>
            {errors.general && <p className="text-danger">{errors.general}</p>}
            <form onSubmit={handleSubmit}>
                <div className="form-group mb-3">
                    <label>Cycle Length</label>
                    <input
                        type="number"
                        name="cycleLength"
                        value={form.cycleLength}
                        onChange={handleChange}
                        className="form-control"
                    />
                </div>

                <div className="form-group mb-3">
                    <label>Last Period Start Date</label>
                    <input
                        type="date"
                        name="lastPeriodStartDate"
                        value={form.lastPeriodStartDate}
                        onChange={handleChange}
                        className="form-control"
                    />
                </div>

                <button type="submit" className="btn btn-primary">
                    Create
                </button>
            </form>
        </div>
    );
};

export default CreateCycleForm;
